using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IStickerRepository : IPendingRepository<Sticker, StickerId>
{
    Task<PagedResult<Sticker>> GetStickersAsync(PagedQuery paged, CancellationToken ct);
    Task<int> CountByGroupAsync(StickerGroupId groupId, CancellationToken ct);
}
