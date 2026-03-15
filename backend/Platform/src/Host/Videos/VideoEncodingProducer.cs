using FitHub.Queue.Contracts.Videos;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Producers;
using Microsoft.Extensions.Options;

namespace FitHub.Host.Videos;

/// <summary>
/// Marker class that carries the [Producer] exchange metadata for RabbitMqProducer.
/// Never instantiated directly — only used as a type parameter for AddProducer.
/// </summary>
[Producer("direct")]
public sealed class VideoEncodingProducer : RabbitMqProducer<VideoEncodingMessage, VideoEncodingProducer, RabbitMqClusterOptions>
{
    public VideoEncodingProducer(IRabbitMqConnection<RabbitMqClusterOptions> connection,
        IOptions<RabbitMqClusterOptions> rabbitOptions,
        ILogger<RabbitMqProducer<VideoEncodingMessage, VideoEncodingProducer, RabbitMqClusterOptions>> logger)
            : base(connection, rabbitOptions, logger)
    {
    }
}
