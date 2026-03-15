using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;

namespace FitHub.Common.EntityFramework;

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
