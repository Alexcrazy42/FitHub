using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.EntityFramework.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        services.AddInterceptors();

        services.AddDbContextPool<TContext>((provider, builder) =>
        {
            var interceptors = provider.GetServices<IInterceptor>();

            builder = options.RequiredDatabaseProvider switch
            {
                DatabaseProvider.PostgreSql => builder.UseNpgsql(
                    options.RequiredConnectionString, optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure();

                }),
                DatabaseProvider.MsSql => builder.UseSqlServer(
                    options.RequiredConnectionString, optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure();
                }),
                _ => throw new UnexpectedException($"Провайдер БД не поддерживается {options.RequiredDatabaseProvider}"),
            };

            builder.UseSnakeCaseNamingConvention()
                .AddInterceptors(interceptors);
        });
        return services;
    }

    public static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddTransient<IInterceptor, AuditableEntitiesInterceptor>();
        services.AddTransient<IInterceptor, UserAuditableEntitiesInterceptor>();
        services.AddTransient<IInterceptor, SoftDeletableEntitiesInterceptor>();
        services.AddTransient<IInterceptor, UserSoftDeletableEntitiesInterceptor>();
        return services;
    }
}
