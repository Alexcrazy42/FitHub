using Testcontainers.PostgreSql;
using Xunit;

namespace FitHub.IntegrationTests.Infrastructure;

public class DbInitializer : IAsyncLifetime
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
