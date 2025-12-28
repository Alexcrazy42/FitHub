using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Identity.Client;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Clients;

public static class ServiceRegistry
{
    public static IServiceCollection AddFitHubClients(this IServiceCollection services)
    {
        services.AddBindedOptions<FitHubClientOptions>();
        services.AddIdentityHttpClient<IChatClient, ChatClient, FitHubClientOptions>();
        services.AddIdentityHttpClient<IMessageClient, MessageClient, FitHubClientOptions>();
        return services;
    }
}
