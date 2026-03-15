using FitHub.Common.EntityFramework;

namespace FitHub.Data;

/// <summary>
/// Опции подключения к базе данных
/// </summary>
public class ConnectionOptions : IDatabaseOptions
{
    public static string SectionName => "Database";

    public string? ConnectionString { get; set; }

    public DatabaseProvider? DatabaseProvider { get; set; }
}
