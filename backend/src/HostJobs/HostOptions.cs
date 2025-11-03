using FitHub.Common.Extensions.Configuration;

namespace FitHub.HostJobs;

public sealed class HostOptions : IHostOptions
{
    public static string SectionName => "Hosting";

    public string? Name { get; set; }
}
