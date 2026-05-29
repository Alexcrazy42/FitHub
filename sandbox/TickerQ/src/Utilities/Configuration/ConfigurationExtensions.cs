using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickerQ.Jobs.Utilities.Objects;

namespace TickerQ.Jobs.Utilities.Configuration;

public static class ConfigurationExtensions
{
    public static T GetRequiredOptions<T>(this IConfiguration configuration)
        where T : class, IHaveConfigSection
        => configuration
            .GetRequiredSection(T.SectionName)
            .Get<T>()
            .Required();

    public static void AddBindedOptions<T>(this IServiceCollection services)
        where T : class, IHaveConfigSection
        => services.AddOptions<T>()
            .BindConfiguration(T.SectionName);
}
