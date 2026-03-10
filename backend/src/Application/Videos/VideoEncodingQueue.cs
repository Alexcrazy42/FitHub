using FitHub.Domain.Videos;
using FitHub.Queue.Contracts.Videos;
using FitHub.RabbitMQ.Producers;

namespace FitHub.Application.Videos;

internal sealed class VideoEncodingQueue : IVideoEncodingQueue
{
    private readonly IRabbitProducer<VideoEncodingMessage> producer;

    public VideoEncodingQueue(IRabbitProducer<VideoEncodingMessage> producer)
    {
        this.producer = producer;
    }

    public Task EnqueueAsync(VideoId id, CancellationToken ct)
    {
        var message = new VideoEncodingMessage { VideoId = id.ToString() };
        return producer.BasicPublishAsync(message, ct);
    }
}
