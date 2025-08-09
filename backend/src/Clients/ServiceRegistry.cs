using FitHub.Clients.Employees;
using FitHub.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Clients;

public static class ServiceRegistry
{
    public static IServiceCollection AddFitHubClients(this IServiceCollection services)
    {
        services.AddBindedOptions<FitHubClientOptions>();
        services.AddHttpClient<IEmployeeClient, EmployeeClient>();
        return services;
    }
}
