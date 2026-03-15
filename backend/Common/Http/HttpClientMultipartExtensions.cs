using System.Collections;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using FitHub.Common.Json;
using Microsoft.AspNetCore.Http;

namespace FitHub.Common.Http;

public static class HttpClientMultipartExtensions
{
    private static readonly JsonSerializerOptions Options = CommonJsonSerializerOptions.Create();

    /// <summary>
    /// Выполнить Post-запрос с <c>Content-type: multipart/form-data</c>
    /// </summary>
    /// <remarks>Парсер AspNet'а не умеет обрабатывать ReadOnly коллекции при десериализации</remarks>
    public static Task<TResponse?> PostAsMultipartAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        Uri url,
        TRequest request,
        CancellationToken cancellationToken = default) =>
        SendAsMultipart<TRequest, TResponse>(httpClient, HttpMethod.Post, url, request, cancellationToken);

    /// <summary>
    /// Выполнить Put-запрос с <c>Content-type: multipart/form-data</c>
    /// </summary>
    /// <remarks>Парсер AspNet'а не умеет обрабатывать ReadOnly коллекции при десериализации</remarks>
    public static Task<TResponse?> PutAsMultipartAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        Uri url,
        TRequest request,
        CancellationToken cancellationToken = default) =>
        SendAsMultipart<TRequest, TResponse>(httpClient, HttpMethod.Put, url, request, cancellationToken);

    /// <summary>
    /// Выполнить Delete-запрос с <c>Content-type: multipart/form-data</c>
    /// </summary>
    /// <remarks>Парсер AspNet'а не умеет обрабатывать ReadOnly коллекции при десериализации</remarks>
    public static Task<TResponse?> DeleteAsMultipartAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        Uri url,
        TRequest request,
        CancellationToken cancellationToken = default) =>
        SendAsMultipart<TRequest, TResponse>(httpClient, HttpMethod.Delete, url, request, cancellationToken);

    private static async Task<TResponse?> SendAsMultipart<TRequest, TResponse>(
        HttpClient httpClient,
        HttpMethod method,
        Uri url,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var content = await BuildMultipartContent(request, cancellationToken);

        using var httpRequest = new HttpRequestMessage(method, url);
        httpRequest.Content = content;
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

        using var resp = await httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        return await resp.ReadFromJsonOrDefaultAsync<TResponse>(cancellationToken);
    }

    private static async Task<MultipartFormDataContent> BuildMultipartContent<TRequest>(TRequest request, CancellationToken ct)
    {
        var content = new MultipartFormDataContent();

        foreach (var prop in typeof(TRequest).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var name = prop.Name;
            var val = prop.GetValue(request);

            switch (val)
            {
                case null:
                    break;

                case IFormFile formFile:
                    await AddFormFile(content, name, formFile, ct);
                    break;

                case IEnumerable<IFormFile> formFiles:
                    foreach (var ff in formFiles)
                    {
                        await AddFormFile(content, name, ff, ct);
                    }
                    break;

                case IEnumerable enumerable when val is not string:
                    var fieldForItems = name + "[]";
                    foreach (var item in enumerable)
                    {
                        switch (item)
                        {
                            case null:
                                break;

                            case IFormFile ifile:
                                await AddFormFile(content, fieldForItems, ifile, ct);
                                break;

                            default:
                                content.Add(new StringContent(ConvertToString(item)), fieldForItems);
                                break;
                        }
                    }
                    break;

                default:
                    content.Add(new StringContent(ConvertToString(val)), name);
                    break;
            }
        }

        return content;
    }

    private static async Task AddFormFile(MultipartFormDataContent content, string fieldName, IFormFile file, CancellationToken ct)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);

        var part = new ByteArrayContent(ms.ToArray());

        var contentType = String.IsNullOrWhiteSpace(file.ContentType)
            ? GetContentType(file.FileName)
            : file.ContentType;

        part.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        content.Add(part, fieldName, file.FileName);
    }

    private static string ConvertToString(object? val) =>
        val switch
        {
            null => String.Empty,
            DateTime dt => dt.ToString("o", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("o", CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => val.ToString() ?? String.Empty
        };

    private static string GetContentType(string fileName)
        => "application/octet-stream";
}
