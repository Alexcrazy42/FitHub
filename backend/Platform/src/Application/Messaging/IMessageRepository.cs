using FitHub.Application.Common;
using FitHub.Application.Messaging.Queries;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IMessageRepository : IPendingRepository<Message, MessageId>
{
    Task<Message> GetMessageAsync(MessageId id, CancellationToken ct = default);

    Task<IReadOnlyList<Message>> GetMessagesAsync(GetMessagesQuery messagesQuery, PagedQuery paged, CancellationToken ct = default);

    Task<IReadOnlyList<Message>> GetUnreadMessagesOlderThan(Message message, IdentityUserId userId, CancellationToken ct = default);
}
