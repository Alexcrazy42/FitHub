using FitHub.Application;
using FitHub.Common.AspNetCore;
using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Logging;
using FitHub.Contracts.V1;
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
        services.AddApplication(configuration);
        services.AddWeb(configuration);

        services.AddExceptionAsProblemDetails();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{typeof(Web.ServiceRegistry).Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
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

        app.UseCors("AllowFrontend");

        app.UseWeb(configuration);

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
            configure.MapControllers().RequireAuthorization();
        });
    }
}
