using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Common.EntityFramework;

public static class ServiceRegistry
{
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services, IDatabaseOptions options)
        where TContext : DbContext
    {
        switch (options.RequiredDatabaseProvider)
        {
            case DatabaseProvider.PostgreSql:
                services.AddScoped<IUnitOfWork, PostgresUnitOfWork<TContext>>();
                break;
            case DatabaseProvider.MsSql:
                services.AddScoped<IUnitOfWork, MsSqlUnitOfWork<TContext>>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(options.RequiredDatabaseProvider), options.RequiredDatabaseProvider, null);
        }

        return services;
    }

    public static IServiceCollection AddDataContext<TContext>(this IServiceCollection services, IDatabaseOptions options)
        where TContext : DbContext
    {
        services.AddDbContextPool<TContext>((provider, builder) =>
        {
            builder = options.RequiredDatabaseProvider switch
            {
                DatabaseProvider.PostgreSql => builder.UseNpgsql(options.RequiredConnectionString, optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure();
                }),
                DatabaseProvider.MsSql => builder.UseSqlServer(options.RequiredConnectionString, optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure();
                }),
                _ => throw new UnexpectedException($"Провайдер БД не поддерживается {options.RequiredDatabaseProvider}"),
            };

            builder.UseSnakeCaseNamingConvention();
        });
        return services;
    }
}
