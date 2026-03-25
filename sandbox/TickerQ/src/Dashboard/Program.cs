using TickerQ.Jobs.Data;

namespace TickerQ.Jobs.Dashboard;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args, b => ConfigureWebHostBuilder(b)).Build();

        try
        {
            await MigrateDatabase(host);

            await host.RunAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private static async Task MigrateDatabase(IHost host)
    {
        await using var dataContext = host.Services.GetRequiredService<TickerDataContext>();
        await dataContext.MigrateAsync();
    }

    private static IHostBuilder CreateHostBuilder(
        string[] args,
        Action<IWebHostBuilder> webHostBuilderConfiguration,
        ServiceProviderOptions? options = null)
        => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .UseServiceProviderFactory(new DefaultServiceProviderFactory(options ?? new ServiceProviderOptions()))
            .ConfigureWebHostDefaults(webHostBuilderConfiguration);

    private static IWebHostBuilder ConfigureWebHostBuilder(this IWebHostBuilder webHostBuilder)
        => webHostBuilder
            .UseStartup<Startup>();
}
