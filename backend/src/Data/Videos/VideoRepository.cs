using FitHub.Application.Common;
using FitHub.Application.Videos;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Videos;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Videos;

public class VideoRepository : DefaultPendingRepository<Video, VideoId, DataContext>, IVideoRepository
{
    public VideoRepository(DataContext context) : base(context) { }

    public async Task<PagedResult<Video>> GetPagedWithResolutionsAsync(PagedQuery query, CancellationToken ct)
    {
        var baseQuery = ReadRaw()
            .Include(v => v.OriginalFile)
            .Include(v => v.Resolutions)
            .OrderBy(v => v.Status == VideoStatus.Pending || v.Status == VideoStatus.Processing ? 0 : 1)
            .ThenByDescending(v => v.CreatedAt);

        var total = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return PagedResult<Video>.Create(items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }

    public async Task<Video?> GetWithResolutionsAsync(VideoId id, CancellationToken ct)
        => await ReadRaw()
            .Include(v => v.OriginalFile)
            .Include(v => v.Resolutions)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task PendingAddResolutionAsync(VideoResolution resolution, CancellationToken ct)
        => await Context.Set<VideoResolution>().AddAsync(resolution, ct);
}
