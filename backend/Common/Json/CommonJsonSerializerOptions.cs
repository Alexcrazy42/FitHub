using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace FitHub.Common.Json;

/// <summary>
/// Общая конфигурация сериализации Json-формата.
/// 1) CamelCase для полей;
/// 2) Поля с null значениями будут проигнорированы;
/// 3) Enum как string
/// 4) Латинские и кириллические символы не эскейпятся
/// </summary>
public static class CommonJsonSerializerOptions
{
    private static readonly JavaScriptEncoder CyrillicEncoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

    /// <inheritdoc cref="CommonJsonSerializerOptions"/>
    public static JsonSerializerOptions Create() => Configure(new JsonSerializerOptions());

    /// <inheritdoc cref="CommonJsonSerializerOptions"/>
    public static JsonSerializerOptions Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Encoder = CyrillicEncoder;

        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
