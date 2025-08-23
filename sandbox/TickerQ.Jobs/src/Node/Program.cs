using TickerQ.Jobs.Data;

namespace TickerQ.Jobs.Node;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(configure =>
                configure.UseStartup<Startup>());

        var host = builder.Build();

        var scope = host.Services.CreateScope();
        await using var dataContext = scope.ServiceProvider.GetRequiredService<TickerDataContext>();
        //await dataContext.MigrateAsync();

        await host.RunAsync();
    }
}
