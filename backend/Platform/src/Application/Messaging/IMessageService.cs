using FitHub.Application.Common;
using FitHub.Application.Messaging.Commands;
using FitHub.Application.Messaging.Queries;
using FitHub.Authentication;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IMessageService
{
    Task<Message> GetMessageAsync(MessageId messageId, CancellationToken ct);

    Task<IReadOnlyList<Message>> GetMessagesAsync(GetMessagesQuery messagesQuery, PagedQuery paged, CancellationToken ct);

    Task<Message> CreateMessageAsync(CreateMessageCommand command, CancellationToken ct);

    Task<Message> UpdateMessageAsync(MessageId id, UpdateMessageCommand command, CancellationToken ct);

    Task<Message> DeleteAsync(MessageId messageId, CancellationToken ct);

    Task<PagedResult<ChatReadingModel>> GetChatReadingsAsync(PagedQuery paged, CancellationToken ct);

    Task ReadMessagesAsync(MessageId messageId, CancellationToken ct);
}
