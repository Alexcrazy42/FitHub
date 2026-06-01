using Microsoft.Extensions.Options;

namespace FitHub.BankManager.Clients.Tests;

internal class TestClient : ITestClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public TestClient(HttpClient client, IOptions<BankManagerClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }


    public async Task<string> Test()
    {
        var uri = new Uri(baseUri, "api/v1/bank/test");
        var response = await client.GetAsync(uri);
        return await response.Content.ReadAsStringAsync();
    }
}
