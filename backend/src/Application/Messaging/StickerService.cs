using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Application.Messaging.Commands;
using FitHub.Application.Messaging.Queries;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Files;
using FitHub.Domain.Messaging;
using FitHub.Shared.Common;

namespace FitHub.Application.Messaging;

internal sealed class StickerService : IStickerService
{
    private readonly IStickerGroupRepository stickerGroupRepository;
    private readonly IStickerRepository stickerRepository;
    private readonly IFileService fileService;
    private readonly IFileRepository fileRepository;
    private readonly IUnitOfWork unitOfWork;

    public StickerService(
        IStickerGroupRepository stickerGroupRepository,
        IStickerRepository stickerRepository,
        IFileService fileService,
        IFileRepository fileRepository,
        IUnitOfWork unitOfWork)
    {
        this.stickerGroupRepository = stickerGroupRepository;
        this.stickerRepository = stickerRepository;
        this.fileService = fileService;
        this.fileRepository = fileRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<StickerGroup> CreateGroupAsync(CreateStickerGroupCommand command, CancellationToken ct)
    {
        var group = StickerGroup.Create(command.Name);
        await stickerGroupRepository.PendingAddAsync(group, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return group;
    }

    public async Task<StickerGroup> UpdateGroupAsync(StickerGroupId id, UpdateStickerGroupCommand command, CancellationToken ct)
    {
        var group = await GetGroupAsync(id, ct);
        group.SetName(command.Name);
        await unitOfWork.SaveChangesAsync(ct);
        return group;
    }

    public async Task<StickerGroup> ActivateGroupAsync(StickerGroupId id, CancellationToken ct)
    {
        var group = await GetGroupAsync(id, ct);
        group.SetIsActive(true);
        await unitOfWork.SaveChangesAsync(ct);
        return group;
    }

    public async Task DeleteGroupAsync(StickerGroupId id, CancellationToken ct)
    {
        var group = await GetGroupAsync(id, ct);

        group.SetIsDeleted(true);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<Sticker> AddStickerAsync(AddStickerCommand command, CancellationToken ct)
    {
        var group = await GetGroupAsync(command.GroupId, ct);

        var file = await fileService.GetFile(command.FileId, ct);
        file.SetEntity(command.GroupId.ToString(), EntityType.Sticker);

        var position = await stickerRepository.CountByGroupAsync(group.Id, ct);
        var sticker = Sticker.Create(command.Name, group, file.Id, position);

        await stickerRepository.PendingAddAsync(sticker, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return sticker;
    }

    public async Task<Sticker> UpdateStickerNameAsync(StickerId id, UpdateStickerNameCommand command, CancellationToken ct)
    {
        var sticker = await GetStickerAsync(id, ct);
        sticker.SetName(command.Name);
        await unitOfWork.SaveChangesAsync(ct);
        return sticker;
    }

    public async Task<Sticker> UpdateStickerPhotoAsync(StickerId id, UpdateStickerPhotoCommand command, CancellationToken ct)
    {
        var sticker = await GetStickerAsync(id, ct);

        var oldFile = await fileService.GetFile(sticker.FileId, ct);
        fileRepository.PendingRemove(oldFile);

        var newFile = await fileService.GetFile(command.NewFileId, ct);
        newFile.SetEntity(sticker.GroupId.ToString(), EntityType.Sticker);
        sticker.SetFile(newFile.Id);

        await unitOfWork.SaveChangesAsync(ct);
        return sticker;
    }

    public async Task RemoveStickerAsync(StickerId id, CancellationToken ct)
    {
        var sticker = await GetStickerAsync(id, ct);
        var file = await fileService.GetFile(sticker.FileId, ct);

        fileRepository.PendingRemove(file);
        stickerRepository.PendingRemove(sticker);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Sticker>> GetStickersByGroupAsync(GetStickersByGroupQuery query, CancellationToken ct)
    {
        await GetGroupAsync(query.GroupId, ct);
        return await stickerRepository.GetAllAsync(x => x.GroupId == query.GroupId, ct);
    }

    public Task<PagedResult<Sticker>> GetStickersAsync(PagedQuery paged, CancellationToken ct)
    {
        return stickerRepository.GetStickersAsync(paged, ct);
    }

    private async Task<StickerGroup> GetGroupAsync(StickerGroupId id, CancellationToken ct)
    {
        var group = await stickerGroupRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(group, "Группа стикеров не найдена!");
        return group;
    }

    private async Task<Sticker> GetStickerAsync(StickerId id, CancellationToken ct)
    {
        var sticker = await stickerRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(sticker, "Стикер не найден!");
        return sticker;
    }
}
