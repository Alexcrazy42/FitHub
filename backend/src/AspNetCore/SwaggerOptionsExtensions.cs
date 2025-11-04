using FitHub.Common.Utilities.System;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace FitHub.Common.AspNetCore;

public static class SwaggerOptionsExtensions
{
    /// <summary>
    /// Добавить Server к конфигурации сваггера из заголовка <c>Referer</c>
    /// </summary>
    /// <remarks>https://www.rfc-editor.org/rfc/rfc9110.html#name-referer</remarks>
    public static void AddRefererServerIfPresent(this SwaggerOptions options)
    {
        options.PreSerializeFilters.Add((swagger, request) =>
        {
            if (!request.Headers.TryGetValue(HeaderNames.Referer, out var value))
            {
                return;
            }

            var externalPath = value
                .Single()
                .Required()
                .RemoveSwaggerSuffixIfPresent();

            swagger.Servers.Add(new OpenApiServer { Url = externalPath });
        });
    }

    private static string RemoveSwaggerSuffixIfPresent(this string input)
        => input.EndsWith("swagger/index.html", StringComparison.InvariantCultureIgnoreCase)
            ? input.Substring(0, input.Length - "swagger/index.html".Length)
            : input;
}
