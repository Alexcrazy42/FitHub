using System.Threading.Channels;
using FitHub.Domain.Videos;

namespace FitHub.Application.Videos;

/// <summary>
/// Unbounded channel that carries VideoIds queued for FFmpeg encoding.
/// Registered as singleton so the BackgroundService and VideoService share the same instance.
/// </summary>
public sealed class VideoEncodingChannel
{
    private readonly Channel<VideoId> _channel =
        Channel.CreateUnbounded<VideoId>(new UnboundedChannelOptions { SingleReader = true });

    public ChannelWriter<VideoId> Writer => _channel.Writer;
    public ChannelReader<VideoId> Reader => _channel.Reader;
}
