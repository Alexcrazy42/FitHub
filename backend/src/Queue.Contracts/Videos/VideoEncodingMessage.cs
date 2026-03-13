using FitHub.RabbitMQ.Contracts;

namespace FitHub.Queue.Contracts.Videos;

public sealed class VideoEncodingMessage : IRabbitMqContract
{
    public static string ExchangeName => "video.encoding";

    public static string ExchangeType => "direct";

    public static string DefaultRoutingKey => "video.encoding.process";

    public string VideoId { get; init; } = String.Empty;
}
