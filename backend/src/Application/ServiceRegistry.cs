using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
