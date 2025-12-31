using FitHub.Application.Common;
using FitHub.Application.Messaging.Commands;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IMessageService
{
    Task<Message> GetMessageAsync(MessageId messageId, CancellationToken ct);

    Task<IReadOnlyList<Message>> GetMessagesAsync(ChatId chatId, PagedQuery paged, CancellationToken ct);

    Task<Message> CreateMessageAsync(CreateMessageCommand command, CancellationToken ct);

    Task<Message> UpdateMessageAsync(MessageId id, UpdateMessageCommand command, CancellationToken ct);

    Task<Message> DeleteAsync(MessageId messageId, CancellationToken ct);
}
