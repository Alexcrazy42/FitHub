namespace FitHub.Clients.Videos;

public interface IVideoClient
{
    Task ProcessAsync(string videoId, CancellationToken ct);
}
