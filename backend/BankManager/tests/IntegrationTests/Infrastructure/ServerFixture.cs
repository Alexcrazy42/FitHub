using FitHub.Authentication;
using FitHub.BankManager.Clients;
using FitHub.BankManager.Clients.Payment;
using FitHub.Common.Testing;
using FitHub.Common.Utilities.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace FitHub.BankManager.IntegrationTests.Infrastructure;

public sealed class ServerFixture : IAsyncLifetime
{
    private TestApplication? testApplication;

    private TestApplication TestApplication => testApplication.Required();

    private IServiceProvider? serviceProvider;

    private IServiceProvider ServiceProvider => serviceProvider.Required();

    // Клиенты нашей апишки
    public IBankManagerPaymentClient PaymentClient => ServiceProvider.GetRequiredService<IBankManagerPaymentClient>();

    // Здесь мокаем зависимости сервиса
    public readonly Mock<IIdentityUserService> IdentityUserServiceMock = new();

    public ServerFixture()
    {
    }

    public async Task InitializeAsync()
    {
        testApplication = new TestApplication(ConfigureTestServices);

        await testApplication.InitializeAsync();

        serviceProvider = BuildConfiguredServiceProvider();
    }

    public async Task DisposeAsync()
    {
        if (testApplication is null)
        {
            return;
        }
        await testApplication.DisposeAsync();
    }

    private IServiceProvider BuildConfiguredServiceProvider()
    {
        var settings = new Dictionary<string, string?>
        {
            {
                $"{BankManagerClientOptions.SectionName}:{nameof(BankManagerClientOptions.ServerUrl)}",
                TestApplication.ClientOptions.BaseAddress.ToString()
            }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var services = new ServiceCollection();

        services.AddTestHostRedirection(TestApplication.Server);

        services.AddTransient<IConfiguration>(_ => configuration);
        services.AddBankManagerClients();

        services.MockIdentityHttpClients();

        return services.BuildServiceProvider();
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services.AddTransient(_ => IdentityUserServiceMock.Object);
        services.MockAuthentication();
    }
}
