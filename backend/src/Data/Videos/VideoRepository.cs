using FitHub.Application.Videos;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Videos;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Videos;

public class VideoRepository : DefaultPendingRepository<Video, VideoId, DataContext>, IVideoRepository
{
    public VideoRepository(DataContext context) : base(context) { }

    public async Task<IReadOnlyList<Video>> GetAllWithResolutionsAsync(CancellationToken ct)
        => await ReadRaw()
            .Include(v => v.OriginalFile)
            .Include(v => v.Resolutions)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(ct);

    public async Task<Video?> GetWithResolutionsAsync(VideoId id, CancellationToken ct)
        => await ReadRaw()
            .Include(v => v.OriginalFile)
            .Include(v => v.Resolutions)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task PendingAddResolutionAsync(VideoResolution resolution, CancellationToken ct)
        => await Context.Set<VideoResolution>().AddAsync(resolution, ct);
}
