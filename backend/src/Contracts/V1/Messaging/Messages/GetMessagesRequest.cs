namespace FitHub.Contracts.V1.Messaging.Messages;

public class GetMessagesRequest
{
    public string? ChatId { get; set; }

    public DateTimeOffset? From { get; set; }

    public bool? IsDescending { get; set; }

    public bool? LoadLastMessages { get; set; }

    public bool? FromUnread { get; set; }
}
