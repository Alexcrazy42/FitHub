using FitHub.Common.Extensions.Configuration;

namespace FitHub.BankManager.Jobs;

public sealed class HostOptions : IHostOptions
{
    public static string SectionName => "Hosting";

    public string? Name { get; set; }
}
