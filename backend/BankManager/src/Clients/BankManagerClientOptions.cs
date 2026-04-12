using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;

namespace FitHub.BankManager.Clients;

public class BankManagerClientOptions : IHaveConfigSection
{
    public static string SectionName => "BankManager";

    public Uri? ServerUrl { get; set; }

    public Uri RequiredServerUrl => ServerUrl.Required();
}
