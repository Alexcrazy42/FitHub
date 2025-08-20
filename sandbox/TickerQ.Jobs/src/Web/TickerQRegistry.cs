using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using TickerQ.Utilities;
using TickerQ.Utilities.Enums;

namespace TickerQ.Jobs.Web;

internal static class TickerQRegistry
{
    public static void AddTickerQHost<TTickerDataContext>(
        this IServiceCollection services,
        HostType hostType,
        Action<TickerOptionsBuilder>? configure = null)
        where TTickerDataContext : DbContext
    {
        services.AddTickerQ(opt =>
        {
            // Define the DbContext to use for storing Tickers.
            opt.AddOperationalStore<TTickerDataContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations(); // Applies custom model customization only during EF Core migrations
                // efOpt.CancelMissedTickersOnApplicationRestart(); // Useful in distributed mode
            }); // Enables EF-backed storage

            configure?.Invoke(opt);
        });
    }

    public static void UseHostTickerQ(this IApplicationBuilder app, HostType hostType)
    {
        switch (hostType)
        {
            case HostType.Node:
                // Node should process jobs immediately
                app.UseTickerQ(TickerQStartMode.Immediate);
                break;

            case HostType.Dashboard:
                // Dashboard should not process jobs at all
                app.UseTickerQ(TickerQStartMode.Manual);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}
