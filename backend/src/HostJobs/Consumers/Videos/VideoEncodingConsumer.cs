using FitHub.Clients.Videos;
using FitHub.Queue.Contracts.Videos;
using FitHub.RabbitMQ.Consumers;

namespace FitHub.HostJobs.Consumers.Videos;

[Consumer("video.encoding.queue", "video.encoding.process")]
public sealed class VideoEncodingConsumer : IRabbitMqConsumerHandler<VideoEncodingMessage>
{
    private readonly IVideoClient videoClient;

    public VideoEncodingConsumer(IVideoClient videoClient)
    {
        this.videoClient = videoClient;
    }

    public Task HandleAsync(VideoEncodingMessage message, CancellationToken ct)
    {
        return videoClient.ProcessAsync(message.VideoId, ct);
    }
}
