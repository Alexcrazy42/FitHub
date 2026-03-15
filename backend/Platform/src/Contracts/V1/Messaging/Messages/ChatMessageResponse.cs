using FitHub.Contracts.V1.Messaging.Chats;

namespace FitHub.Contracts.V1.Messaging.Messages;

public class ChatMessageResponse
{
    public string? Id { get; set; }

    public ChatResponse? Chat { get; set; }

    public MessageResponse? LastMessage { get; set; }

    public int? UnreadCount { get; set; }

    public DateTimeOffset? LastMessageTime { get; set; }
}
