namespace FitHub.Contracts.V1.Messaging.Chats;

public class InitiatorAndTargetUserRequest
{
    public string? ChatId { get; set; }

    public string? TargetUserId { get; set; }
}
