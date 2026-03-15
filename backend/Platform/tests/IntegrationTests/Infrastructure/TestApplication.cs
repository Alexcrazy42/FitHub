using FitHub.Common.EntityFramework;
using FitHub.Data;
using FitHub.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

public sealed class TestApplication : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private const string ServiceName = "FitHub";
    private const string ContainerPrefixPrefix = $"{ServiceName}_ContainerTests";

    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage(DockerImages.PostgreSql)
        .WithUsername("dbUser")
        .WithPassword("dbPassword")
        .WithDatabase(database: ServiceName)
        .WithName($"{ContainerPrefixPrefix}_PostgreSql_{Guid.NewGuid()}")
        .Build();

    public Action<IServiceCollection>? ConfigureTestServices { get; set; }

    public TestApplication(Action<IServiceCollection>? configureTestServices = null)
    {
        ConfigureTestServices = configureTestServices;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbConnectionStringEnvName = $"{ConnectionOptions.SectionName}:{nameof(ConnectionOptions.ConnectionString)}";
        var databaseProviderEnvName = $"{ConnectionOptions.SectionName}:{nameof(ConnectionOptions.DatabaseProvider)}";

        Environment.SetEnvironmentVariable(
            dbConnectionStringEnvName,
            dbContainer.GetConnectionString(),
            EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable(
            databaseProviderEnvName,
            nameof(DatabaseProvider.PostgreSql),
            EnvironmentVariableTarget.Process);

        var configurationValues = new Dictionary<string, string?>
        {
            { dbConnectionStringEnvName, dbContainer.GetConnectionString() },
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
        return dbContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        await dbContainer.StopAsync();
    }


    async Task IAsyncLifetime.DisposeAsync()
        => await DisposeAsync();

}
