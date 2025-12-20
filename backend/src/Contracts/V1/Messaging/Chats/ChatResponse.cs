using FitHub.Shared.Messaging;

namespace FitHub.Contracts.V1.Messaging.Chats;

public sealed class ChatResponse
{
    public string? Id { get; set; }

    public ChatType? Type { get; set; }

    public List<ChatParticipantResponse> Participants { get; set; } = [];
}
