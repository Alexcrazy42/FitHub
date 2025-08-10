using FitHub.Common.EntityFramework;
using FitHub.Common.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Data;

public static class ServiceRegistry
{
    /// <summary>
    /// Добавить сервисы слоя данных (БД)
    /// </summary>
    /// <remarks>
    /// Подключение к бд будет происходить по
    /// </remarks>
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
        return services;
    }
}
