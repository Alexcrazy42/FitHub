using FitHub.Contracts.V1.Users;

namespace FitHub.Contracts.V1.Messaging;

public class ChatParticipantResponse
{
    public string? Id { get; set; }

    public string? ChatId { get; set; }

    public UserResponse? User { get; set; }

    public DateTimeOffset? JoinedAt { get; set; }
}
