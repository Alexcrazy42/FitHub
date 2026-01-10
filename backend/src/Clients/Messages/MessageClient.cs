using FitHub.Common.Http;
using FitHub.Contracts;
using FitHub.Contracts.V1.Messaging.Messages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FitHub.Clients.Messages;

internal sealed class MessageClient : IMessageClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public MessageClient(HttpClient client, IOptions<FitHubClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }

    public async Task<ListResponse<MessageResponse>> GetMessagesAsync(GetMessagesRequest request, PagedRequest paged, CancellationToken ct)
    {
        var url = "api/v1/messages";

        var queryParams = new Dictionary<string, string?>
        {
            ["ChatId"] = request.ChatId,
            ["From"] = request.From?.ToString("O"), // ISO 8601
            ["IsDescending"] = request.IsDescending?.ToString(),
            ["PageSize"] = paged.PageSize.ToString(),
            ["PageNumber"] = paged.PageNumber.ToString()
        };

        url = QueryHelpers.AddQueryString(url,
            queryParams.Where(kvp => kvp.Value != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!)!);

        var fullUrl = new Uri(baseUri, url);

        return await client.GetFromJsonOrDefaultAsync<ListResponse<MessageResponse>>(fullUrl, ct)
               ?? new ListResponse<MessageResponse>();
    }

    public Task<MessageResponse?> CreateMessageAsync(CreateMessageRequest request, CancellationToken ct)
    {
        var url = new Uri(baseUri, "api/v1/messages");
        return client.PostAsJsonAsync<CreateMessageRequest, MessageResponse>(url, request, ct);
    }

    public Task<MessageResponse?> UpdateMessageAsync(string id, UpdateMessageRequest request, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"api/v1/messages/{id}");
        return client.PutAsJsonAsync<UpdateMessageRequest, MessageResponse>(url, request, ct);
    }

    public async Task DeleteMessageAsync(string id, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"api/v1/messages/{id}");
        var response = await client.DeleteAsync(url, ct);
        response.EnsureSuccessStatusCode();
    }


}
