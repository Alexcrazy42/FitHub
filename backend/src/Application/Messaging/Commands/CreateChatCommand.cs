using FitHub.Authentication;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Messaging;
using FitHub.Shared.Messaging;

namespace FitHub.Application.Messaging.Commands;

public class CreateChatCommand
{
    public ChatType Type { get; set; }

    public List<IdentityUserId> ParticipantUserIds { get; set; }

    public CreateChatCommand(ChatType type, List<IdentityUserId> participantUserIds)
    {
        Type = type;
        ParticipantUserIds = participantUserIds;
    }
}

public static class CreateChatCommandExtensions
{
    public static CreateChatCommand FromRequest(this CreateChatRequest request)
    {
        var participantUserIds = request.ParticipantUserIds.Select(IdentityUserId.Parse).ToList();
        return new CreateChatCommand(request.Type.Required(), participantUserIds);
    }
}
