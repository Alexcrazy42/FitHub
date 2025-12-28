using FitHub.Application.Common;
using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Commands;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Messaging.Messages;
using FitHub.Domain.Messaging;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Messaging;

public class MessageController : ControllerBase
{
    private readonly IMessageService messageService;

    public MessageController(IMessageService messageService)
    {
        this.messageService = messageService;
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

        return message.ToResponse();
    }

    [HttpPut(ApiRoutesV1.MessagesById)]
    public async Task<MessageResponse> UpdateMessageAsync([FromRoute] string? id, [FromBody] UpdateMessageRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request);
        var messageId = MessageId.Parse(id);
        var command = request.ToCommand();
        var message = await messageService.UpdateMessageAsync(messageId, command, ct);
        return message.ToResponse();
    }

    [HttpDelete(ApiRoutesV1.MessagesById)]
    public async Task DeleteMessageAsync([FromRoute] string? id, CancellationToken ct)
    {
        var messageId = MessageId.Parse(id);
        await messageService.DeleteAsync(messageId, ct);
    }
}
