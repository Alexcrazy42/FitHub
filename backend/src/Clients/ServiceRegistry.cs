using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using FitHub.Common.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Clients;

public static class ServiceRegistry
{
    public static IServiceCollection AddFitHubClients(this IServiceCollection services)
    {
        services.AddBindedOptions<FitHubClientOptions>();
        services.AddHttpClient<IChatClient, ChatClient>();
        services.AddHttpClient<IMessageClient, MessageClient>();
        return services;
    }
}
