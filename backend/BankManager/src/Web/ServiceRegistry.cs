using Microsoft.Extensions.DependencyInjection;

namespace FitHub.BankManager.Web;

public static class ServiceRegistry
{
    public static IServiceCollection AddBankManagerWeb(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(ServiceRegistry).Assembly);

        return services;
    }
}
