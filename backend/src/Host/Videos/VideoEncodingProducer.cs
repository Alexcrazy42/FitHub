using FitHub.RabbitMQ.Contracts.Videos;
using FitHub.RabbitMQ.Producers;

namespace FitHub.Host.Videos;

/// <summary>
/// Marker class that carries the [Producer] exchange metadata for RabbitMqProducer.
/// Never instantiated directly — only used as a type parameter for AddProducer.
/// </summary>
[Producer("direct")]
public sealed class VideoEncodingProducer : IRabbitProducer<VideoEncodingMessage>
{
    public Task BasicPublishAsync(VideoEncodingMessage message, CancellationToken ct = default)
        => throw new NotSupportedException("Not called directly. Resolved via RabbitMqProducer.");
}
