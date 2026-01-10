using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IMessageViewRepository : IPendingRepository<MessageView, MessageViewId>
{
    Task<Message?> GetFirstUnreadMessageAsync(ChatId chatId, IdentityUserId userId, CancellationToken ct);
}
