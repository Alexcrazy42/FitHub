using FitHub.Application.Common;
using FitHub.Application.Messaging.Commands;
using FitHub.Application.Messaging.Queries;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IStickerService
{
    // Commands: StickerGroup
    Task<StickerGroup> CreateGroupAsync(CreateStickerGroupCommand command, CancellationToken ct);
    Task<StickerGroup> UpdateGroupAsync(StickerGroupId id, UpdateStickerGroupCommand command, CancellationToken ct);
    Task<StickerGroup> ActivateGroupAsync(StickerGroupId id, CancellationToken ct);
    Task DeleteGroupAsync(StickerGroupId id, CancellationToken ct);

    // Commands: Sticker
    Task<Sticker> AddStickerAsync(AddStickerCommand command, CancellationToken ct);
    Task<Sticker> UpdateStickerNameAsync(StickerId id, UpdateStickerNameCommand command, CancellationToken ct);
    Task<Sticker> UpdateStickerPhotoAsync(StickerId id, UpdateStickerPhotoCommand command, CancellationToken ct);
    Task RemoveStickerAsync(StickerId id, CancellationToken ct);

    // Queries
    Task<IReadOnlyList<Sticker>> GetStickersByGroupAsync(GetStickersByGroupQuery query, CancellationToken ct);
    Task<PagedResult<Sticker>> GetStickersAsync(PagedQuery paged, CancellationToken ct);
}
