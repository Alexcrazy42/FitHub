using FitHub.Extensions.Configuration;
using FitHub.Utilities.System;

namespace FitHub.Clients;

public class FitHubClientOptions : IHaveConfigSection
{
    public static string SectionName => "FitHub";

    public Uri? ServerUrl { get; set; }

    public Uri RequiredServerUrl => ServerUrl.Required();
}
