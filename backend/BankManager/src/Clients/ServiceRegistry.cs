using FitHub.BankManager.Clients.Payment;
using FitHub.BankManager.Clients.Tests;
using FitHub.Common.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.BankManager.Clients;

public static class ServiceRegistry
{
    public static IServiceCollection AddBankManagerClients(this IServiceCollection services)
    {
        services.AddBindedOptions<BankManagerClientOptions>();
        services.AddHttpClient<IBankManagerPaymentClient, BankManagerPaymentClient>();
        services.AddHttpClient<ITestClient, TestClient>();
        return services;
    }
}
