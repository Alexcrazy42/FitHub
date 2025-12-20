using FitHub.Shared.Messaging;

namespace FitHub.Contracts.V1.Messaging.Chats;

public class CreateChatRequest
{
    public ChatType? Type { get; set; }

    public List<string> ParticipantUserIds { get; set; } = [];
}
