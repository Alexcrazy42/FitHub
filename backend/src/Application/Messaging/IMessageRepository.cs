using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IMessageRepository : IPendingRepository<Message, MessageId>
{
    Task<Message> GetMessageAsync(MessageId id, CancellationToken ct = default);

    Task<IReadOnlyList<Message>> GetMessagesAsync(ChatId chatId, PagedQuery paged, CancellationToken ct = default);
}
