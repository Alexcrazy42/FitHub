namespace FitHub.RabbitMQ.Contracts.Videos;

public sealed class VideoEncodingMessage : IRabbitMqContract
{
    public string VideoId { get; init; } = string.Empty;

    public static string ExchangeName => "video.encoding";

    public static string DefaultRoutingKey => "video.encoding.process";

    public static string FallbackRoutingKey => "video.encoding.process.fallback";
}
