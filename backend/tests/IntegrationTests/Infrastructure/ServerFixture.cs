using FitHub.Clients;
using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using FitHub.Common.Testing;
using FitHub.Common.Utilities.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

public sealed class ServerFixture : IAsyncLifetime
{
    private TestApplication? testApplication;

    private TestApplication TestApplication => testApplication.Required();

    private IServiceProvider? serviceProvider;

    private IServiceProvider ServiceProvider => serviceProvider.Required();

    // Клиенты нашей апишки
    public IChatClient ChatClient => ServiceProvider.GetRequiredService<IChatClient>();
    public IMessageClient MessageClient => ServiceProvider.GetRequiredService<IMessageClient>();

    // Здесь мокаем внешние зависимости сервиса
    // public Mock<ISomeExternalServiceClient> SomeExternalServiceClient { get; } = TestApplication.Services.GetRequiredService<ISomeExternalServiceClient>();

    public async Task InitializeAsync()
    {
        testApplication = new TestApplication();

        await testApplication.InitializeAsync();

        serviceProvider = BuildConfiguredServiceProvider();
    }

    public async Task DisposeAsync()
    {
        if (testApplication == null)
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
                $"{FitHubClientOptions.SectionName}:{nameof(FitHubClientOptions.ServerUrl)}",
                TestApplication.ClientOptions.BaseAddress.ToString()
            }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var services = new ServiceCollection();

        services.AddTestHostRedirection(TestApplication.Server);

        services.AddTransient<IConfiguration>(_ => configuration);
        services.AddFitHubClients();

        return services.BuildServiceProvider();
    }
}
