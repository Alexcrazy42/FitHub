using FitHub.Data;
using FitHub.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

public sealed class TestApplication : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private Action<IServiceCollection>? ConfigureTestServices { get; set; }
    private DbInitializer DbInitializer { get; set; }


    public TestApplication(Action<IServiceCollection>? configureTestServices = null)
    {
        ConfigureTestServices = configureTestServices;
        DbInitializer = new DbInitializer();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbConnectionStringEnvName = $"{ConnectionOptions.SectionName}:{nameof(ConnectionOptions.ConnectionString)}";

        var configurationValues = new Dictionary<string, string?>
        {
            { dbConnectionStringEnvName, DbInitializer.ConnectionString },
        };

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(configurationValues);
        });

        builder.ConfigureTestServices(services =>
        {
            ConfigureTestServices?.Invoke(services);
        });
    }

    public Task InitializeAsync()
    {
        return DbInitializer.InitializeAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        await DbInitializer.DisposeAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
        => await DisposeAsync();

}
