using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.BankManager.Data;

public static class ServiceRegistry
{
    public static IServiceCollection AddBankManagerData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Database:ConnectionString"];

        if (String.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database:ConnectionString is required for BankManager.");
        }

        services.AddDbContextPool<BankManagerDataContext>(builder =>
        {
            builder.UseNpgsql(connectionString, options => options.EnableRetryOnFailure())
                .UseSnakeCaseNamingConvention();
        });
        services.AddScoped<IBankManagerUnitOfWork, BankManagerUnitOfWork>();
        services.AddScoped<IPaymentIntentRepository, PaymentIntentRepository>();
        services.AddScoped<IBankManagerOutboxRepository, RabbitOutboxRepository>();

        return services;
    }
}
