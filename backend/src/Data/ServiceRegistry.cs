using System.Reflection;
using FitHub.Common.Entities.Storage;
using FitHub.Common.EntityFramework;
using FitHub.Common.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Data;

public static class ServiceRegistry
{
    /// <summary>
    /// Добавить сервисы слоя данных
    /// </summary>
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBindedOptions<ConnectionOptions>();

        var databaseOptions = configuration.GetRequiredOptions<ConnectionOptions>();
        services.AddDataContext<DataContext>(databaseOptions);
        services.AddUnitOfWork<DataContext>(databaseOptions);

        services.AddRepositories();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var applicationAssembly = typeof(Application.ServiceRegistry).Assembly;
        var repositoryAssembly = typeof(ServiceRegistry).Assembly;

        var pendingRepoInterfaces = applicationAssembly.GetTypes()
            .Where(t => t.IsInterface &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IPendingRepository<,>)))
            .ToList();

        foreach (var interfaceType in pendingRepoInterfaces)
        {
            var implementation = repositoryAssembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

            if (implementation != null)
            {
                services.AddTransient(interfaceType, implementation);
            }
        }

        return services;
    }
}
