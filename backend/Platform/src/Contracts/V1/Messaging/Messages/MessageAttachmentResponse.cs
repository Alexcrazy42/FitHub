using FitHub.Contracts.V1.Users;
using FitHub.Shared.Messaging;

namespace FitHub.Contracts.V1.Messaging.Messages;

public class MessageAttachmentResponse
{
    public string? Id { get; set; }

    public string? MessageId { get; set; }

    public MessageAttachmentType? Type { get; set; }

    public string? Data { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public UserResponse? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public UserResponse? UpdatedBy { get; set; }
}
