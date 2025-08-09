using Microsoft.Extensions.Options;

namespace FitHub.Clients.Employees;

public class EmployeeClient : IEmployeeClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public EmployeeClient(HttpClient client, IOptions<FitHubClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }
}
