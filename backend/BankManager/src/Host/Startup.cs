using FitHub.BankManager.Application;
using FitHub.BankManager.Data;
using FitHub.BankManager.Web;
using FitHub.Common.AspNetCore;
using FitHub.Common.Logging;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;

namespace FitHub.BankManager.Host;

public class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBankManagerData(configuration);
        services.AddBankManagerApplication();
        services.AddBankManagerWeb();

        services.AddExceptionAsProblemDetails();

        services.AddRabbitMq<RabbitMqClusterOptions>();
        services.AddBasicProducer<RabbitMqClusterOptions>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{typeof(Web.ServiceRegistry).Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCommonRequestLogging();

        app.UseStatusCodePages();

        var isDev = env.IsDevelopment();
        app.UseExceptionAsProblemDetails(isDev);
        app.UseHttpsRedirection();
        app.UseRouting();


        if (isDev)
        {
            app.UseSwagger(options =>
            {
                options.AddRefererServerIfPresent();
            });
            app.UseSwaggerUI(options =>
            {
            });
        }

        app.UseEndpoints(configure =>
        {
            configure.MapControllers();
        });
    }
}
