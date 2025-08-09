using FitHub.Extensions.Configuration;
using FitHub.Utilities.System;

namespace FitHub.EntityFramework;

/// <summary>
/// Опции базы данных
/// </summary>
public interface IDatabaseOptions : IHaveConfigSection
{
    static abstract string IHaveConfigSection.SectionName { get; }

    public string? ConnectionString { get; }
    public string RequiredConnectionString => ConnectionString.Required();

    public DatabaseProvider? DatabaseProvider { get; }
    public DatabaseProvider RequiredDatabaseProvider => DatabaseProvider.Required();
}
