using Testcontainers.PostgreSql;
using Xunit;

namespace FitHub.BankManager.IntegrationTests.Infrastructure;

public class DbInitializer : IAsyncLifetime
{
    private const string ServiceName = "FitHub.BankManager";
    private const string ContainerPrefixPrefix = $"{ServiceName}_ContainerTests";
    private const string DockerPgImageName = "postgres:17.0-alpine";

    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage(DockerPgImageName)
        .WithUsername("dbUser")
        .WithPassword("dbPassword")
        .WithDatabase(database: ServiceName)
        .WithName($"{ContainerPrefixPrefix}_PostgreSql_{Guid.NewGuid()}")
        .Build();

    public string ConnectionString => dbContainer.GetConnectionString();
    public Task InitializeAsync()
    {
        return dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await dbContainer.StopAsync();
    }
}
