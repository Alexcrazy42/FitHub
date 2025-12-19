using FitHub.Shared.Messaging;

namespace FitHub.Contracts.V1.Messaging;

public class CreateChatRequest
{
    public ChatType? Type { get; set; }

    public List<string> ParticipantUserIds { get; set; } = [];
}
