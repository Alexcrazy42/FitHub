using System.Text.Json;

namespace FitHub.Common.Json;

/// <summary>
/// Сериализатор Json с общими опциями <see cref="CommonJsonSerializerOptions"/>
/// </summary>
public static class CommonJsonSerializer
{
    private static readonly JsonSerializerOptions Options = CommonJsonSerializerOptions.Create();

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    public static T? Deserialize<T>(string state) => JsonSerializer.Deserialize<T>(state, Options);
}

