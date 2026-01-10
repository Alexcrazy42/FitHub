using FitHub.Common.Http;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Messaging.Chats;
using Microsoft.Extensions.Options;

namespace FitHub.Clients.Chats;

internal sealed class ChatClient : IChatClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public ChatClient(HttpClient client, IOptions<FitHubClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }

    public Task<ChatResponse?> GetChatAsync(string chatId, CancellationToken ct)
    {
        var url = new Uri(baseUri, $"/chats/{chatId}");
        return client.GetFromJsonOrDefaultAsync<ChatResponse>(url, ct);
    }

    public Task<ChatResponse?> CreateChatAsync(CreateChatRequest request, CancellationToken ct)
    {
        var url = new Uri(baseUri, ApiRoutesV1.Chats);
        return client.PostAsJsonAsync<CreateChatRequest, ChatResponse>(url, request, ct);
    }
}
