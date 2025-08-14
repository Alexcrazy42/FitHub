using FitHub.Application;
using FitHub.Common.AspNetCore;
using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Logging;
using FitHub.Data;
using FitHub.Web;

namespace FitHub.Host;

public sealed class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBindedOptions<HostOptions>();
        services.AddData(configuration);
        services.AddApplication();
        services.AddWeb(configuration);

        services.AddExceptionAsProblemDetails();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{typeof(Web.ServiceRegistry).Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStatusCodePages();

        app.UseCommonRequestLogging();

        var isProduction = env.IsProduction();
        if (!isProduction)
        {
            app.UseSwagger(options =>
            {
                options.AddRefererServerIfPresent();
            });
            app.UseSwaggerUI();
        }

        app.UseExceptionAsProblemDetails(!isProduction);

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseEndpoints(opt => opt.MapControllers());
    }
}
