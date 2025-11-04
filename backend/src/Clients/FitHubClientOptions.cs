using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;

namespace FitHub.Clients;

public class FitHubClientOptions : IHaveConfigSection
{
    public static string SectionName => "FitHub";

    public Uri? ServerUrl { get; set; }

    public Uri RequiredServerUrl => ServerUrl.Required();
}
