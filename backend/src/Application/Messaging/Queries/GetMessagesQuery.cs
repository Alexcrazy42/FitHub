using FitHub.Common.Entities;
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
        if (fromUnread == false && from == null)
        {
            throw new ValidationException("При поиске не от непрочитанного сообщения надо указать параметр от какого времени ищем!");
        }

        ChatId = chatId;
        From = from;
        FromUnread = fromUnread;
        IsDescending = isDescending;
    }
}
