using FitHub.Application.Messaging.Commands;
using FitHub.Authentication;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IChatService
{
    Task<Chat> GetChatAsync(ChatId id, CancellationToken ct);

    Task<Chat> CreateChatAsync(CreateChatCommand command, CancellationToken ct);

    Task<Message> InviteUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct);

    Task<Message> ExcludeUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct);

    Task<IReadOnlyList<Chat>> GetUserChatsAsync(IdentityUserId userId, CancellationToken ct);
}
