using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace FitHub.Common.Testing;

public static class ServiceRegistry
{
    public static void AddTestHostRedirection(this IServiceCollection services, TestServer server)
    {
        services.AddSingleton(server);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, TestServerMessageFilter>());
    }
}
