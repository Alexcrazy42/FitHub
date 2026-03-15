using FitHub.Common.Utilities.System;

namespace FitHub.Common.Extensions.Configuration;

/// <summary>
/// Настройки хоста
/// </summary>
public interface IHostOptions : IHaveConfigSection
{
    static abstract string IHaveConfigSection.SectionName { get; }

    /// <summary>
    /// Имя сервиса
    /// </summary>
    public string? Name { get; set; }

    public string RequiredName => Name.Required();
}
