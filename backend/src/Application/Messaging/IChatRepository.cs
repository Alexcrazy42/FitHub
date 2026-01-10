using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IChatRepository : IPendingRepository<Chat, ChatId>
{
    Task<Chat> GetAsync(ChatId id, CancellationToken ct = default);

    Task<Chat?> GetFirstOrDefaultOneToOneChatAsync(List<IdentityUserId> participantUserIds, CancellationToken ct = default);

    Task<IReadOnlyList<Chat>> GetUserChatsAsync(IdentityUserId userId, CancellationToken ct = default);
}
