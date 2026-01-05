using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Common.Entities;
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

    [HttpGet(ApiRoutesV1.ChatMessagesList)]
    public async Task<ListResponse<ChatMessageResponse>> GetChatMessageList([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        var pagedQuery = paged.ToQuery();

        var chatReadingModelResult = await messageService.GetChatReadingsAsync(pagedQuery, ct);

        return chatReadingModelResult.ToListResponse(MessagesExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.MessagesRead)]
    public async Task MessageReadAsync([FromBody] MessageReadRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, nameof(request));
        var maxMessageId = MessageId.Parse(request.MaxMessageId);

        await messageService.ReadMessagesAsync(maxMessageId, ct);
    }

    [HttpPost(ApiRoutesV1.Messages)]
    public async Task<MessageResponse> CreateMessageAsync([FromBody] CreateMessageRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request);
        var command = request.ToCommand();
        var message = await messageService.CreateMessageAsync(command, ct);
        var response = message.ToResponse();

        _ = Task.Run(async () =>
        {
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.All
                .CreateMessage(response);
            await chatHubContext.Clients.Group(groupName)
                .CreateMessage(response);
        }, ct);

        return response;
    }

    [HttpPut(ApiRoutesV1.MessagesById)]
    public async Task<MessageResponse> UpdateMessageAsync([FromRoute] string? id, [FromBody] UpdateMessageRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request);
        var messageId = MessageId.Parse(id);
        var command = request.ToCommand();
        var message = await messageService.UpdateMessageAsync(messageId, command, ct);

        var response = message.ToResponse();

        _ = Task.Run(async () =>
        {
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .UpdateMessage(response);
        }, ct);

        return response;
    }

    [HttpDelete(ApiRoutesV1.MessagesById)]
    public async Task DeleteMessageAsync([FromRoute] string? id, CancellationToken ct)
    {
        var messageId = MessageId.Parse(id);
        var message = await messageService.DeleteAsync(messageId, ct);

        _ = Task.Run(async () =>
        {
            var groupName = message.ChatId.ToString().GetChatGroupName();

            await chatHubContext.Clients.Group(groupName)
                .MessageDeleted(message.ChatId.ToString(), messageId.ToString());
        }, ct);
    }
}
