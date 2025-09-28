using System.Text.Json.Serialization;
using FitHub.Common.AspNetCore;
using FitHub.Common.Extensions.Configuration;
using FitHub.Web.V1.Trainings.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Web;

public static class ServiceRegistry
{
    public static void AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBindedOptions<AuthOptions>();
        var authOptions = configuration.GetRequiredOptions<AuthOptions>();
        services.AddJwtAuthenticationAndAuthorization<AuthOptions>(authOptions);

        services.AddValidations();

        services
            .AddMvcCore()
            .AddApiExplorer()
            .AddApplicationPart(typeof(Web.ServiceRegistry).Assembly)
            .AddControllersAsServices().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddExceptionAsProblemDetails();
        services.AddSignalR();
    }

    public static void UseWeb(this IApplicationBuilder app, IConfiguration configuration)
    {
        var authOptions = configuration.GetRequiredOptions<AuthOptions>();
        app.UseAuthenticationAndAuthorization(authOptions);
    }

    private static void AddValidations(this IServiceCollection services)
    {
        // нельзя раскоментировать эту строчку, иначе ломается ответ ProblemDetails заготовленный
        //services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true);

        var webAssembly = typeof(Web.ServiceRegistry).Assembly;
        services.AddValidatorsFromAssembly(webAssembly);
    }
}
