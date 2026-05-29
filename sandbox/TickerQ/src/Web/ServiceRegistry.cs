using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.EntityFrameworkCore;
using TickerQ.Utilities;

namespace TickerQ.Jobs.Web;

public static class ServiceRegistry
{
    public static void AddWeb<TTickerDataContext>(
        this IServiceCollection services,
        HostType hostType,
        Action<TickerOptionsBuilder>? configure = null,
        Action<EfCoreOptionBuilder>? configureStore = null)
        where TTickerDataContext : DbContext
    {
        services.AddTickerQHost<TTickerDataContext>(hostType, configure, configureStore);
    }

    public static void UseWeb(this IApplicationBuilder app, HostType hostType)
    {
        app.UseHostTickerQ(hostType);
    }
}
