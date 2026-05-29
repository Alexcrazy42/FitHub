using TickerQ.Jobs.Utilities.Configuration;
using TickerQ.Jobs.Utilities.Objects;

namespace TickerQ.Jobs.Data;

internal sealed class ConnectionOptions : IHaveConfigSection
{
    public static string SectionName => "Database";

    public string? TickerConnectionString { get; set; }

    public string RequiredTickerConnectionString => TickerConnectionString.Required();
}
