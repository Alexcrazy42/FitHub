using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;

namespace FitHub.Common.Logging;

public sealed class LogstashOptions : IHaveConfigSection
{
    public static string SectionName => "Logstash";

    public string? HttpAddress { get; set; }

    public string RequiredHttpAddress => HttpAddress.Required();
}
