using FitHub.Common.AspNetCore.Tokens;
using FitHub.Common.Identity.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FitHub.Common.Testing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection MockAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            foreach (var scheme in options.Schemes)
            {
                scheme.HandlerType = typeof(TestAuthenticationHandler);
            }
        });

        services.Replace(new ServiceDescriptor(
            serviceType: typeof(ITokenService),
            implementationType: typeof(TestTokenService),
            ServiceLifetime.Singleton));

        return services;
    }

    public static void MockIdentityHttpClients(this IServiceCollection services)
    {
        services.TryAddTransient<ITokenService, TestTokenService>();
        services.Replace(new ServiceDescriptor(
            serviceType: typeof(ClientIdentityHandlerBase),
            implementationType: typeof(TestClientIdentityHandler),
            ServiceLifetime.Transient));
    }
}
