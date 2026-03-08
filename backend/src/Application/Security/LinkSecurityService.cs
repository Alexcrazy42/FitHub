using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FitHub.Application.Security;

public class LinkSecurityService : ILinkSecurityService
{
    private readonly ILogger<LinkSecurityService> logger;

    public LinkSecurityService(
        ILogger<LinkSecurityService> logger)
    {
        this.logger = logger;
    }

    public async Task<LinkSecurityCheckResult> CheckUrlAsync(string url, CancellationToken ct)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return new LinkSecurityCheckResult("unknown", false, 0, 0, 0, SecurityStatus.Unknown);
        }

        await Task.Delay(1000, ct);

        return new LinkSecurityCheckResult(url, true, 0, 0, 0, SecurityStatus.Safe);

        // var domain = uri.Host;
        // var hasSsl = string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase);
        //
        // if (string.IsNullOrWhiteSpace(apiKey))
        //     return new LinkSecurityCheckResult(domain, hasSsl, 0, 0, 0, SecurityStatus.Unknown);
        //
        // try
        // {
        //     var client = httpClientFactory.CreateClient();
        //     client.DefaultRequestHeaders.Clear();
        //     client.DefaultRequestHeaders.Add("x-apikey", apiKey);
        //
        //     // Submit URL for scanning
        //     var body = new FormUrlEncodedContent([new KeyValuePair<string, string>("url", url)]);
        //     var submitResp = await client.PostAsync("https://www.virustotal.com/api/v3/urls", body, ct);
        //
        //     if (!submitResp.IsSuccessStatusCode)
        //         return new LinkSecurityCheckResult(domain, hasSsl, 0, 0, 0, SecurityStatus.Unknown);
        //
        //     var submit = await submitResp.Content.ReadFromJsonAsync<VtSubmitResponse>(cancellationToken: ct);
        //     if (submit?.Data?.Id is null)
        //         return new LinkSecurityCheckResult(domain, hasSsl, 0, 0, 0, SecurityStatus.Unknown);
        //
        //     // Brief wait for VirusTotal to complete the scan
        //     await Task.Delay(2000, ct);
        //
        //     var analysisResp = await client.GetAsync(
        //         $"https://www.virustotal.com/api/v3/analyses/{submit.Data.Id}", ct);
        //
        //     if (!analysisResp.IsSuccessStatusCode)
        //         return new LinkSecurityCheckResult(domain, hasSsl, 0, 0, 0, SecurityStatus.Unknown);
        //
        //     var analysis = await analysisResp.Content.ReadFromJsonAsync<VtAnalysisResponse>(cancellationToken: ct);
        //     var stats = analysis?.Data?.Attributes?.Stats;
        //
        //     if (stats is null)
        //         return new LinkSecurityCheckResult(domain, hasSsl, 0, 0, 0, SecurityStatus.Unknown);
        //
        //     var total = stats.Malicious + stats.Suspicious + stats.Undetected + stats.Harmless + stats.Timeout;
        //     var status = stats.Malicious > 0 ? SecurityStatus.Malicious
        //         : stats.Suspicious > 0 ? SecurityStatus.Suspicious
        //         : SecurityStatus.Safe;
        //
        //     return new LinkSecurityCheckResult(domain, hasSsl, stats.Malicious, stats.Suspicious, total, status);
        // }
        // catch (Exception ex)
        // {
        //     logger.LogWarning(ex, "VirusTotal check failed for {Url}", url);
        //     return new LinkSecurityCheckResult(domain, hasSsl, 0, 0, 0, SecurityStatus.Unknown);
        // }
    }
}

// VirusTotal JSON response models
file record VtSubmitData([property: JsonPropertyName("id")] string? Id);
file record VtAnalysisResponse([property: JsonPropertyName("data")] VtAnalysisData? Data);
file record VtAnalysisData([property: JsonPropertyName("attributes")] VtAnalysisAttributes? Attributes);
file record VtAnalysisAttributes([property: JsonPropertyName("stats")] VtStats? Stats);
file record VtStats(
    [property: JsonPropertyName("malicious")] int Malicious,
    [property: JsonPropertyName("suspicious")] int Suspicious,
    [property: JsonPropertyName("undetected")] int Undetected,
    [property: JsonPropertyName("harmless")] int Harmless,
    [property: JsonPropertyName("timeout")] int Timeout);
