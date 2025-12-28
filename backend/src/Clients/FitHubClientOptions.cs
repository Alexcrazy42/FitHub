using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Identity.Client;
using FitHub.Common.Utilities.System;

namespace FitHub.Clients;

public class FitHubClientOptions : IClientIdentityOptions
{
    public static string SectionName => "FitHub";

    public Uri? ServerUrl { get; set; }

    public string? AccessToken { get; set; }

    public Uri RequiredServerUrl => ServerUrl.Required();
}
