using FitHub.Common.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FitHub.Common.Identity.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddIdentityHttpClient<TClient, TImplementation, TIdentityOptions>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
        where TIdentityOptions : class, IClientIdentityOptions
    {
        services.TryAddIdentityClients<TIdentityOptions>();

        return services.AddHttpClient<TClient, TImplementation>()
            .AddHttpMessageHandler<ClientIdentityHandlerBase>();
    }

    internal static bool TryAddIdentityClients<TIdentityOptions>(this IServiceCollection services)
        where TIdentityOptions : class, IClientIdentityOptions
    {
        if (!services.TryAddOnceSingleton<IdentityRegistredMarker>())
        {
            return false;
        }

        services.AddBindedOptionsAs<TIdentityOptions, IClientIdentityOptions>();

        services.TryAddTransient<ClientIdentityHandlerBase, ClientIdentityHandler>();
        return true;
    }

    internal static bool TryAddOnceSingleton<T>(this IServiceCollection services)
        where T : class
    {
        var isRegistered = services.Any(s => s.ServiceType == typeof(T));
        if (isRegistered)
        {
            return false;
        }

        services.AddSingleton<T>();
        return true;
    }

    private sealed class IdentityRegistredMarker;
}
