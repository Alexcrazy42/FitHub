using FitHub.Application.Messaging.Commands;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging;

public interface IChatService
{
    Task<Chat> GetChatAsync(ChatId id, CancellationToken ct);

    Task<Chat> CreateChatAsync(CreateChatCommand command, CancellationToken ct);

    Task InviteUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct);

    Task ExcludeUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct);
}
