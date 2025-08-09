using FitHub.Extensions.Configuration;

namespace FitHub.Host;

public sealed class HostOptions : IHostOptions
{
    public static string SectionName => "Hosting";

    public string? Name { get; set; }
}
