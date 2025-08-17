using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(Application.ServiceRegistry).Assembly;

        var interfaces = applicationAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.ToLower().EndsWith("service"))
            .ToList();

        foreach (var interfaceType in interfaces)
        {
            var implementation = applicationAssembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

            if (implementation != null)
            {
                services.AddTransient(interfaceType, implementation);
            }
        }

        services.AddTransient<IIdentityUserService, IdentityUserService>();
        services.AddTransient<IAuthenticationService, IdentityUserService>();

        return services;
    }
}
