using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FitHub.Common.Json;

namespace FitHub.Common.Http;

public static class HttpClientJsonExtensions
{
    private static readonly JsonSerializerOptions Options = CommonJsonSerializerOptions.Create();

    /// <summary>
    /// Отправить <c>GET</c> запрос по адресу <c>requestUri</c> и получить ответ <c>TResponse</c>
    /// </summary>
    public static async Task<TResponse?> GetFromJsonOrDefaultAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправить <c>POST</c> запрос с телом <c>body</c> по адресу <c>requestUri</c> и получить ответ <c>TResponse</c>
    /// </summary>
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(requestUri, body, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправить <c>POST</c> запрос по адресу <c>requestUri</c> и получить ответ <c>TResponse</c>
    /// </summary>
    public static async Task<TResponse?> PostAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправить <c>PUT</c> запрос с телом <c>body</c> по адресу <c>requestUri</c> и получить ответ <c>TResponse</c>
    /// </summary>
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PutAsJsonAsync(requestUri, body, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправить <c>DELETE</c> запрос с телом <c>body</c> по адресу <c>requestUri</c> и получить ответ <see cref="HttpResponseMessage"/>
    /// </summary>
    public static Task<HttpResponseMessage> DeleteAsJsonAsync<TRequest>(
        this HttpClient client,
        Uri? requestUri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        request.Content = JsonContent.Create(body);

        return client.SendAsync(request, cancellationToken);
    }

    internal static Task<TResponse?> ReadFromJsonOrDefaultAsync<TResponse>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        response.EnsureSuccessStatusCode();

        // Стандартный метод `ReadFromJsonAsync` бросает исключение, если встречает пустой Content
        if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
        {
            // No content
            return Task.FromResult<TResponse?>(default);
        }

        return response.Content.ReadFromJsonAsync<TResponse>(Options, cancellationToken: cancellationToken);
    }
}
