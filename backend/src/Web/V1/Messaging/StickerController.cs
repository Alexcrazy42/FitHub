using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Application.Messaging.Queries;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Messaging.Stickers;
using FitHub.Domain.Files;
using FitHub.Domain.Messaging;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Messaging;

[ApiController]
[Authorize]
public class StickerController : ControllerBase
{
    private readonly IStickerService stickerService;
    private readonly IStickerGroupRepository stickerGroupRepository;

    public StickerController(IStickerService stickerService, IStickerGroupRepository stickerGroupRepository)
    {
        this.stickerService = stickerService;
        this.stickerGroupRepository = stickerGroupRepository;
    }

    // ── StickerGroups ────────────────────────────────────────────────────────

    [HttpGet(ApiRoutesV1.StickerGroups)]
    public async Task<ListResponse<StickerGroupResponse>> GetGroupsAsync(CancellationToken ct)
    {
        var groups = await stickerGroupRepository.GetAllAsync(x => !x.IsDeleted, ct);
        return groups.ToListResponse(StickerResponseExtensions.ToResponse);
    }

    [HttpGet(ApiRoutesV1.StickerGroupStickers)]
    public async Task<ListResponse<StickerResponse>> GetStickersByGroupAsync([FromRoute] string? id, CancellationToken ct)
    {
        var groupId = StickerGroupId.Parse(id);
        var stickers = await stickerService.GetStickersByGroupAsync(new GetStickersByGroupQuery(groupId), ct);
        return stickers.ToListResponse(StickerResponseExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.StickerGroups)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<StickerGroupResponse> CreateGroupAsync([FromBody] CreateStickerGroupRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request");
        var name = ValidationException.ThrowIfNull(request.Name, "Name");
        var group = await stickerService.CreateGroupAsync(new CreateStickerGroupCommand(name), ct);
        return group.ToResponse();
    }

    [HttpPut(ApiRoutesV1.StickerGroupById)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<StickerGroupResponse> UpdateGroupAsync([FromRoute] string? id, [FromBody] UpdateStickerGroupRequest? request, CancellationToken ct)
    {
        var groupId = StickerGroupId.Parse(id);
        request = ValidationException.ThrowIfNull(request, "request");
        var name = ValidationException.ThrowIfNull(request.Name, "Name");
        var group = await stickerService.UpdateGroupAsync(groupId, new UpdateStickerGroupCommand(name), ct);
        return group.ToResponse();
    }

    [HttpPost(ApiRoutesV1.StickerGroupActivate)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<StickerGroupResponse> ActivateGroupAsync([FromRoute] string? id, CancellationToken ct)
    {
        var groupId = StickerGroupId.Parse(id);
        var group = await stickerService.ActivateGroupAsync(groupId, ct);
        return group.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.StickerGroupById)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task DeleteGroupAsync([FromRoute] string? id, CancellationToken ct)
    {
        var groupId = StickerGroupId.Parse(id);
        await stickerService.DeleteGroupAsync(groupId, ct);
    }

    // ── Stickers ─────────────────────────────────────────────────────────────

    [HttpGet(ApiRoutesV1.Stickers)]
    public async Task<ListResponse<StickerResponse>> GetStickersAsync([FromQuery] PagedRequest? request, CancellationToken ct)
    {
        var query = request.ToQuery();
        var pagedResult = await stickerService.GetStickersAsync(query, ct);
        return pagedResult.ToListResponse(StickerResponseExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.Stickers)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<StickerResponse> AddStickerAsync([FromBody] AddStickerRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request");
        var groupId = StickerGroupId.Parse(ValidationException.ThrowIfNull(request.GroupId, "GroupId"));
        var fileId = FileId.Parse(ValidationException.ThrowIfNull(request.FileId, "FileId"));
        var name = ValidationException.ThrowIfNull(request.Name, "Name");
        var sticker = await stickerService.AddStickerAsync(new AddStickerCommand(groupId, fileId, name), ct);
        return sticker.ToResponse();
    }

    [HttpPut(ApiRoutesV1.StickerName)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<StickerResponse> UpdateStickerNameAsync([FromRoute] string? id, [FromBody] UpdateStickerNameRequest? request, CancellationToken ct)
    {
        var stickerId = StickerId.Parse(id);
        request = ValidationException.ThrowIfNull(request, "request");
        var name = ValidationException.ThrowIfNull(request.Name, "Name");
        var sticker = await stickerService.UpdateStickerNameAsync(stickerId, new UpdateStickerNameCommand(name), ct);
        return sticker.ToResponse();
    }

    [HttpPut(ApiRoutesV1.StickerPhoto)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<StickerResponse> UpdateStickerPhotoAsync([FromRoute] string? id, [FromBody] UpdateStickerPhotoRequest? request, CancellationToken ct)
    {
        var stickerId = StickerId.Parse(id);
        request = ValidationException.ThrowIfNull(request, "request");
        var newFileId = FileId.Parse(ValidationException.ThrowIfNull(request.NewFileId, "NewFileId"));
        var sticker = await stickerService.UpdateStickerPhotoAsync(stickerId, new UpdateStickerPhotoCommand(newFileId), ct);
        return sticker.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.StickerById)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task RemoveStickerAsync([FromRoute] string? id, CancellationToken ct)
    {
        var stickerId = StickerId.Parse(id);
        await stickerService.RemoveStickerAsync(stickerId, ct);
    }
}
