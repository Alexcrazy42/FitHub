using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace FitHub.AspNetCore;

public static class HttpResponseExtensions
{
    private const string DefaultContentType = "application/octet-stream";

    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new FileExtensionContentTypeProvider();

    /// <summary>
    /// Установить необходимые хэдера в <see cref="HttpResponse"/> для ответа файлом
    /// </summary>
    /// <param name="response">Ответ HTTP, в который необходимо записать хэдера</param>
    /// <param name="fileName">Имя возвращаемого файла (вместе с расширением при его наличии)</param>
    /// <param name="contentLength">Размер файла, опционально</param>
    public static void SetFileHeaders(this HttpResponse response, string fileName, int? contentLength = null)
    {
        response.SetContentType(fileName);
        response.SetContentDispositionHeader(fileName);

        if (contentLength.HasValue)
        {
            response.SetContentLength(contentLength.Value);
        }
    }

    private static void SetContentLength(this HttpResponse response, int contentLength)
        => response.Headers.ContentLength = contentLength;

    private static void SetContentType(this HttpResponse response, string fileName)
    {
        if (!ContentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = DefaultContentType;
        }

        response.Headers.ContentType = contentType;
    }

    private static void SetContentDispositionHeader(this HttpResponse response, string fileName)
    {
        var contentDisposition = new ContentDispositionHeaderValue(DispositionTypeNames.Attachment);
        contentDisposition.SetHttpFileName(fileName);

        response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
    }
}
