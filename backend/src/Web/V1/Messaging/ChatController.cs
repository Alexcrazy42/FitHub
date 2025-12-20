using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Messaging;
using FitHub.Contracts.V1.Messaging.Chats;
using FitHub.Domain.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Messaging;

[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService chatService;

    public ChatController(IChatService chatService)
    {
        this.chatService = chatService;
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
}
