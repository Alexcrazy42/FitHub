using FitHub.Application.Common;
using FitHub.Application.Messaging;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Messaging;

public class StickerRepository : DefaultPendingRepository<Sticker, StickerId, DataContext>, IStickerRepository
{
    public StickerRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<Sticker> ReadRaw()
    {
        return DbSet
            .Include(x => x.Group)
            .Include(x => x.File);
    }

    public async Task<PagedResult<Sticker>> GetStickersAsync(PagedQuery paged, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        var total = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(x => x.Position)
            .Skip((paged.PageNumber - 1) * paged.PageSize)
            .Take(paged.PageSize)
            .ToListAsync(ct);

        return PagedResult<Sticker>.Create(items, totalItems: total, currentPage: paged.PageNumber, pageSize: paged.PageSize);
    }

    public Task<int> CountByGroupAsync(StickerGroupId groupId, CancellationToken ct)
    {
        return DbSet.CountAsync(x => x.GroupId == groupId, ct);
    }
}
