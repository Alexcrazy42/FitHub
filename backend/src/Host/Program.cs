using FitHub.Common.Logging;
using FitHub.Data;
using Serilog;

namespace FitHub.Host;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args, b => ConfigureWebHostBuilder(b)).Build();

        try
        {
            Log.Information("Запуск приложения");
            await MigrateDatabase(host);

            // if (args.contains(MigrateAndExitKey))
            //{
            //    return; // Выходим из приложения, тк нас попросили только применить миграцию
            //}

            await host.RunAsync();
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Приложение неожиданно остановилось");
            throw;
        }
        finally
        {
            Log.Information("Остановка приложения...");
            await Log.CloseAndFlushAsync();
        }
    }

    private static async Task MigrateDatabase(IHost host)
    {
        await using var dataContext = host.Services.GetRequiredService<DataContext>();
        await dataContext.MigrateAsync();
    }

    private static IHostBuilder CreateHostBuilder(
        string[] args,
        Action<IWebHostBuilder> webHostBuilderConfiguration,
        ServiceProviderOptions? options = null)
        => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .UseCommonLogger<HostOptions>()
            .UseServiceProviderFactory(new DefaultServiceProviderFactory(options ?? new ServiceProviderOptions()))
            .ConfigureWebHostDefaults(webHostBuilderConfiguration);

    private static IWebHostBuilder ConfigureWebHostBuilder(this IWebHostBuilder webHostBuilder)
        => webHostBuilder
            .UseStartup<Startup>();
}
