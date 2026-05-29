using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.Jobs.Utilities.Configuration;

namespace TickerQ.Jobs.Data;

public static class ServiceRegistry
{
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBindedOptions<ConnectionOptions>();
        var options = configuration.GetRequiredOptions<ConnectionOptions>();

        services.AddDbContext<TickerDataContext>(builder =>
        {
            builder.UseNpgsql(options.RequiredTickerConnectionString, optionsBuilder =>
            {
                optionsBuilder.EnableRetryOnFailure();
            });
        });
    }
}
