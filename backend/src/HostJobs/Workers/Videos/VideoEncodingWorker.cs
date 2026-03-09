using FitHub.Application.Videos;
using FitHub.Domain.Videos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FitHub.HostJobs.Workers.Videos;

public class VideoEncodingWorker : BackgroundService
{
    private readonly VideoEncodingChannel _channel;
    private readonly IServiceProvider _provider;
    private readonly ILogger<VideoEncodingWorker> _logger;

    public VideoEncodingWorker(
        VideoEncodingChannel channel,
        IServiceProvider provider,
        ILogger<VideoEncodingWorker> logger)
    {
        _channel = channel;
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Worker} starting", nameof(VideoEncodingWorker));

        await foreach (var videoId in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Picked up encoding job for video {VideoId}", videoId);
            try
            {
                await using var scope = _provider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<IVideoService>();
                await service.ProcessAsync(videoId, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unhandled error encoding video {VideoId}", videoId);
            }
        }

        _logger.LogInformation("{Worker} stopping", nameof(VideoEncodingWorker));
    }
}
