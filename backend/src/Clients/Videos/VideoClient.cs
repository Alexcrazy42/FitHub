using Microsoft.Extensions.Options;

namespace FitHub.Clients.Videos;

internal sealed class VideoClient : IVideoClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public VideoClient(HttpClient client, IOptions<FitHubClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }

    public async Task ProcessAsync(string videoId, CancellationToken ct)
    {
        var oldTimeout = client.Timeout;
        client.Timeout = Timeout.InfiniteTimeSpan;

        try
        {
            var url = new Uri(baseUri, $"/api/v1/videos/{videoId}/process");
            var response = await client.PostAsync(url, content: null, ct);
            response.EnsureSuccessStatusCode();
        }
        finally
        {
            client.Timeout = oldTimeout;  // восстанавливаем
        }
    }
}
