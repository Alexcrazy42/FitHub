namespace FitHub.Common.Extensions.Configuration;

/// <summary>
/// Имеется секция в конфиге
/// </summary>
public interface IHaveConfigSection
{
    /// <summary>
    /// Ключ секции в конфиге
    /// </summary>
    static abstract string SectionName { get; }
}
