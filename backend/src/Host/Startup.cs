using FitHub.Application;
using FitHub.Common.AspNetCore;
using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Logging;
using FitHub.Contracts.V1;
using FitHub.Data;
using FitHub.Web;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

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
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FitHub API v1",
                Version = "v1",
                Description = "An ASP.NET Core Web API for managing ToDo items",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Example Contact",
                    Url = new Uri("https://example.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });

            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "FitHub API v2",
                Version = "v2"
            });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                var relativePath = apiDesc.RelativePath?.ToLowerInvariant();
                if (string.IsNullOrEmpty(relativePath))
                {
                    return false;
                }

                return docName switch
                {
                    "v1" => relativePath.StartsWith("api/v1/"),
                    "v2" => relativePath.StartsWith("api/v2/"),
                    _ => false
                };
            });

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

        app.UseWeb(configuration);

        if (isDev)
        {
            app.UseSwagger(options =>
            {
                options.AddRefererServerIfPresent();
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FitHub API v1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "FitHub API v2");
            });
        }

        app.UseEndpoints(configure =>
        {
            configure.MapControllers().RequireAuthorization();
        });
    }
}
