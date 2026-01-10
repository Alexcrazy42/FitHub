using FitHub.Authentication;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Commands;

public class InitiatorAndTargetUserCommand
{
    public ChatId ChatId { get; init; }

    public IdentityUserId InitiatorUserId { get; init; }

    public IdentityUserId TargetUserId { get; init; }

    public InitiatorAndTargetUserCommand(ChatId chatId, IdentityUserId initiatorUserId, IdentityUserId targetUserId)
    {
        ChatId = chatId;
        InitiatorUserId = initiatorUserId;
        TargetUserId = targetUserId;
    }
}
