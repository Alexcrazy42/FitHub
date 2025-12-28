using FitHub.Authentication;
using FitHub.Clients;
using FitHub.Clients.Chats;
using FitHub.Clients.Messages;
using FitHub.Common.Testing;
using FitHub.Common.Utilities.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

public sealed class ServerFixture : IAsyncLifetime
{
    private TestApplication? testApplication;

    private TestApplication TestApplication => testApplication.Required();

    private IServiceProvider? serviceProvider;

    private IServiceProvider ServiceProvider => serviceProvider.Required();

    public DataSeed DataSeed { get; }

    public CurrentUserProvider CurrentUserProvider { get; }

    // Клиенты нашей апишки
    public IChatClient ChatClient => ServiceProvider.GetRequiredService<IChatClient>();
    public IMessageClient MessageClient => ServiceProvider.GetRequiredService<IMessageClient>();

    // Здесь мокаем зависимости сервиса
    public readonly Mock<IIdentityUserService> IdentityUserServiceMock = new Mock<IIdentityUserService>();

    public ServerFixture()
    {
        DataSeed = new DataSeed();
        CurrentUserProvider = new CurrentUserProvider(DataSeed.AllUsers.First(), IdentityUserServiceMock);
    }

    public async Task InitializeAsync()
    {
        testApplication = new TestApplication(ConfigureTestServices);

        await testApplication.InitializeAsync();

        await DataSeed.SeedDataAsync(testApplication);

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

        services.AddSingleton<ITestDesiredUserIdProvider, CurrentUserProvider>(_ => CurrentUserProvider);
        services.MockIdentityHttpClients();

        return services.BuildServiceProvider();
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services.AddTransient(_ => IdentityUserServiceMock.Object);
        services.MockAuthentication();
    }
}
