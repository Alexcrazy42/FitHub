using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Queries;

public class GetMessagesQuery
{
    public ChatId ChatId { get; init; }

    public DateTimeOffset? From { get; init; }

    public bool IsDescending { get; init; }

    public bool FromUnread { get; init; }

    public GetMessagesQuery(ChatId chatId, bool isDescending, bool fromUnread, DateTimeOffset? from = null)
    {
        ChatId = chatId;
        From = from;
        FromUnread = fromUnread;
        IsDescending = isDescending;
    }
}
