using FitHub.Utilities.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FitHub.Extensions.Configuration;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Получить конфигурацию
    /// </summary>
    public static IConfiguration GetConfiguration(this IServiceProvider provider)
        => provider.GetRequiredService<IConfiguration>();

    /// <summary>
    /// Получить опции из конфига
    /// </summary>
    public static T GetRequiredOptions<T>(this IConfiguration configuration)
        where T : class, IHaveConfigSection
        => configuration
            .GetRequiredSection(T.SectionName)
            .Get<T>()
            .Required();

    /// <summary>
    /// Добавить (зарегистрировать) опции
    /// </summary>
    public static void AddBindedOptions<T>(this IServiceCollection services)
        where T : class, IHaveConfigSection
        => services.AddOptions<T>()
            .BindConfiguration(T.SectionName);

    /// <summary>
    /// Добавить (зарегистрировать) опции <see cref="TOptions"/> как <see cref="TAsOptions"/>
    /// </summary>
    /// <remarks>Опции <see cref="TAsOptions"/> должны быть зарегестрированные в контейнере заранее</remarks>
    public static void AddBindedOptionsAs<TOptions, TAsOptions>(this IServiceCollection services)
        where TOptions : class, TAsOptions
        where TAsOptions : class, IHaveConfigSection
        => services.AddSingleton<IOptions<TAsOptions>>(serviceProvider =>
            Options.Create<TAsOptions>(serviceProvider.GetRequiredService<IOptions<TOptions>>().Value));

}
