using System.Security.Claims;
using FitHub.Host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FitHub.UnitTests.Di;

public class ContainerFixture : WebApplicationFactory<IApiMarker>
{
    public IServiceProvider Container => Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "SkipMigration", "true" },
                { "Database:ConnectionString", "_" },
                { "Database:DatabaseProvider", "PostgreSql" }
            });
        })
        .ConfigureTestServices(services =>
        {
            // Мокируем HttpContextAccessor, ибо в тестовом окружении не существует HttpContext
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new DefaultHttpContext();

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            services.AddSingleton(mockHttpContextAccessor.Object);
        });
    }
}
