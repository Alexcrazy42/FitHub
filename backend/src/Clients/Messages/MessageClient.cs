using FitHub.Common.Http;
using FitHub.Contracts;
using FitHub.Contracts.V1.Messaging.Messages;
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

    public async Task<ListResponse<MessageResponse>> GetMessagesAsync(string chatId, PagedRequest paged, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"/messages?chatId={chatId}&PageSize={paged.PageSize}&PageNumber={paged.PageNumber}");
        return await client.GetFromJsonOrDefaultAsync<ListResponse<MessageResponse>>(url, ct)
            ?? new ListResponse<MessageResponse>();
    }

    public Task<MessageResponse?> CreateMessageAsync(CreateMessageRequest request, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"/messages");
        return client.PostAsJsonAsync<CreateMessageRequest, MessageResponse>(url, request, ct);
    }

    public Task<MessageResponse?> UpdateMessageAsync(string id, UpdateMessageRequest request, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"/messages/{id}");
        return client.PutAsJsonAsync<UpdateMessageRequest, MessageResponse>(url, request, ct);
    }

    public async Task DeleteMessageAsync(string id, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"/messages/{id}");
        var response = await client.DeleteAsync(url, ct);
        response.EnsureSuccessStatusCode();
    }
}
