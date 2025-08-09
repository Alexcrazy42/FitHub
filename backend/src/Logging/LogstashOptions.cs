using FitHub.Extensions.Configuration;
using FitHub.Utilities.System;

namespace FitHub.Logging;

public sealed class LogstashOptions : IHaveConfigSection
{
    public static string SectionName => "Logstash";

    public string? HttpAddress { get; set; }

    public string RequiredHttpAddress => HttpAddress.Required();
}
