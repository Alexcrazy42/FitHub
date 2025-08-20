using TickerQ.Jobs.Application;
using TickerQ.Jobs.Data;
using TickerQ.Jobs.Web;

namespace TickerQ.Jobs.Node;

public sealed class Startup
{
    private const HostType HostType = Web.HostType.Node;

    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddData(configuration);
        services.AddApplication();
        services.AddWeb<TickerDataContext>(HostType, configureTicker =>
        {
            var instanceIdentifier = Guid.NewGuid().ToString();
            Console.WriteLine(instanceIdentifier);
            configureTicker.SetInstanceIdentifier(instanceIdentifier);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWeb(HostType);
    }
}
