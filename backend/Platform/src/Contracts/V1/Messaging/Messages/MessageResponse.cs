using FitHub.Contracts.V1.Users;

namespace FitHub.Contracts.V1.Messaging.Messages;

public class MessageResponse
{
    public string? Id { get; set; }

    public string? ChatId { get; set; }

    public string? MessageText { get; set; }

    public MessageResponse? ReplyMessage { get; set; }

    public MessageResponse? ForwardedMessage { get; set; }

    public List<MessageAttachmentResponse> Attachments { get; set; } = [];

    public DateTimeOffset? CreatedAt { get; set; }

    public UserResponse? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public UserResponse? UpdatedBy { get; set; }
}
