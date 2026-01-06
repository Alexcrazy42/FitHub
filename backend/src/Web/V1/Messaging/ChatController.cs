using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.Domain.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Web.V1.Messaging;

[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService chatService;
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;
    private readonly IHubContext<ChatHub, IChatHub> chatHubContext;

    public ChatController(IChatService chatService, ICurrentIdentityUserIdAccessor userIdAccessor,
        IHubContext<ChatHub, IChatHub> chatHubContext)
    {
        this.chatService = chatService;
        this.userIdAccessor = userIdAccessor;
        this.chatHubContext = chatHubContext;
    }

    [HttpGet(ApiRoutesV1.ChatById)]
    public async Task<ChatResponse> GetByIdAsync([FromRoute] string? id, CancellationToken ct)
    {
        var chatId = ChatId.Parse(id);
        var chat = await chatService.GetChatAsync(chatId, ct);
        return chat.ToResponse();
    }

    [HttpPost(ApiRoutesV1.Chats)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<ChatResponse> CreateChat([FromBody] CreateChatRequest? request, CancellationToken ct)
    {
        var command = UnexpectedException.ThrowIfNull(request, "request").FromRequest();
        var chat = await chatService.CreateChatAsync(command, ct);
        return chat.ToResponse();
    }

    [HttpPost(ApiRoutesV1.ChatInvite)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task InviteToChatAsync([FromBody] InitiatorAndTargetUserRequest? request, CancellationToken ct)
    {
        var command = ValidationException.ThrowIfNull(request, "request").FromRequest(userIdAccessor.GetCurrentUserId());
        var message = await chatService.InviteUserAsync(command, ct);

        var response = message.ToResponse();

        _ = Task.Run(async () =>
        {
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .CreateMessage(response);
        }, ct);
    }

    [HttpPost(ApiRoutesV1.ChatExclude)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task ExcludeFromChatAsync([FromBody] InitiatorAndTargetUserRequest? request, CancellationToken ct)
    {
        var command = ValidationException.ThrowIfNull(request, "request").FromRequest(userIdAccessor.GetCurrentUserId());
        var message = await chatService.ExcludeUserAsync(command, ct);

        var response = message.ToResponse();

        _ = Task.Run(async () =>
        {
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .CreateMessage(response);
        }, ct);

        // TODO: убрать человека из группы этого чата, при этом надо обязательно дождаться ответа,
        // иначе ему будут приходить уведомления о сообщениях чата, откуда его выкинули (нужно обдумать этот подход)
    }
}
