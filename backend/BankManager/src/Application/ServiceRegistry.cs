using FitHub.BankManager.Application.Payments;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.BankManager.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddBankManagerApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentIntentService, PaymentIntentService>();
        services.AddScoped<IBankManagerOutboxPublisherService, BankManagerOutboxPublisherService>();
        return services;
    }
}
