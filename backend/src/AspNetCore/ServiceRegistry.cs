using FitHub.Common.AspNetCore.Problems;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Common.AspNetCore;

public static class ServiceRegistry
{
    public static void AddExceptionAsProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionLoggingHandler>()
            .AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSingleton<IExceptionToProblemRegistry, CommonExceptionToProblemRegistry>();
    }

    /// <summary>
    /// Включить обработку исключений как <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>
    /// </summary>
    public static IApplicationBuilder UseExceptionAsProblemDetails(this IApplicationBuilder app, bool useExceptionPage)
    {
        if (useExceptionPage)
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler();

        return app;
    }
}
