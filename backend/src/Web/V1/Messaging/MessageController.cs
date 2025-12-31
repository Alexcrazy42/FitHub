using FitHub.Application.Common;
using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Common.AspNetCore;
using FitHub.Common.Entities;
using FitHub.Common.Utilities.System;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Messaging.Messages;
using FitHub.Domain.Messaging;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Web.V1.Messaging;

public class MessageController : ControllerBase
{
    private readonly IMessageService messageService;
    private readonly IHubContext<ChatHub, IChatHub> chatHubContext;

    public MessageController(IMessageService messageService, IHubContext<ChatHub, IChatHub> chatHubContext)
    {
        this.messageService = messageService;
        this.chatHubContext = chatHubContext;
    }

    [HttpGet(ApiRoutesV1.Messages)]
    public async Task<ListResponse<MessageResponse>> GetMessagesAsync([FromQuery] string? chatId,
        [FromQuery] PagedRequest? paged,
        CancellationToken ct)
    {
        var parsedChatId = ChatId.Parse(ValidationException.ThrowIfNull(chatId));
        var pagedQuery = paged.ToQuery();

        var messages = await messageService.GetMessagesAsync(parsedChatId, pagedQuery, ct);
        return messages.ToListResponse(MessagesExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.Messages)]
    public async Task<MessageResponse> CreateMessageAsync([FromBody] CreateMessageRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request);
        var command = request.ToCommand();
        var message = await messageService.CreateMessageAsync(command, ct);

        _ = Task.Run(async () =>
        {
            var userName = HttpContext.User.GetUsername().Required();
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .CreateMessage(userName, message.MessageText, DateTime.UtcNow);
        }, ct);

        return message.ToResponse();
    }

    [HttpPut(ApiRoutesV1.MessagesById)]
    public async Task<MessageResponse> UpdateMessageAsync([FromRoute] string? id, [FromBody] UpdateMessageRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request);
        var messageId = MessageId.Parse(id);
        var command = request.ToCommand();
        var message = await messageService.UpdateMessageAsync(messageId, command, ct);

        _ = Task.Run(async () =>
        {
            var userName = HttpContext.User.GetUsername().Required();
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .UpdateMessage(userName, message.MessageText, DateTime.UtcNow);
        }, ct);

        return message.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.MessagesById)]
    public async Task DeleteMessageAsync([FromRoute] string? id, CancellationToken ct)
    {
        var messageId = MessageId.Parse(id);
        var message = await messageService.DeleteAsync(messageId, ct);

        _ = Task.Run(async () =>
        {
            //var userName = HttpContext.User.GetUsername().Required();
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .MessageDeleted(messageId.ToString());
        }, ct);
    }
}
