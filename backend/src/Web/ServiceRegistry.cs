using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Web;

public static class ServiceRegistry
{
    public static void AddWeb(this IServiceCollection services)
    {

        services
            .AddMvcCore()
            .AddApiExplorer()
            .AddApplicationPart(typeof(Web.ServiceRegistry).Assembly)
            .AddControllersAsServices();
    }
}
