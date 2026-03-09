using FitHub.Clients.Videos;
using FitHub.RabbitMQ.Consumers;
using FitHub.RabbitMQ.Contracts.Videos;

namespace FitHub.HostJobs.Workers.Videos;

[Consumer("video.encoding.queue", "video.encoding.process")]
public sealed class VideoEncodingConsumer : IRabbitMqConsumerHandler<VideoEncodingMessage>
{
    private readonly IVideoClient videoClient;

    public VideoEncodingConsumer(IVideoClient videoClient)
    {
        this.videoClient = videoClient;
    }

    public Task HandleAsync(VideoEncodingMessage message, CancellationToken ct)
        => videoClient.ProcessAsync(message.VideoId, ct);
}
