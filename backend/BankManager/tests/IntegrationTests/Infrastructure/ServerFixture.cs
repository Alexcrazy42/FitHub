using FitHub.Authentication;
using FitHub.BankManager.Application.Mocks;
using FitHub.BankManager.Clients;
using FitHub.BankManager.Clients.Payment;
using FitHub.BankManager.Clients.Tests;
using FitHub.BankManager.Data;
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

    // нужные сервис из TestApplication
    public BankManagerDataContext DataContext => TestApplication.Services.GetRequiredService<BankManagerDataContext>();

    // Клиенты нашей апишки
    public IBankManagerPaymentClient PaymentClient => ServiceProvider.GetRequiredService<IBankManagerPaymentClient>();
    public ITestClient TestClient => ServiceProvider.GetRequiredService<ITestClient>();

    // Здесь мокаем зависимости сервиса
    public Mock<IMockTestService> MockTestService = new Mock<IMockTestService>();

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
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();

        services.AddTestHostRedirection(TestApplication.Server);

        services.AddTransient<IConfiguration>(_ => configuration);
        services.AddBankManagerClients();

        return services.BuildServiceProvider();
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services.AddScoped(_ => MockTestService.Object);
    }
}
