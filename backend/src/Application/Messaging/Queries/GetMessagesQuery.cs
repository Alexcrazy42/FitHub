using FitHub.Common.Entities;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Queries;

public class GetMessagesQuery
{
    public ChatId ChatId { get; init; }

    public DateTimeOffset? From { get; init; }

    public bool IsDescending { get; init; }

    public bool LoadLastMessages { get; init; }

    public bool FromUnread { get; init; }

    public GetMessagesQuery(ChatId chatId,
        bool isDescending,
        bool loadLastMessages,
        bool fromUnread,
        DateTimeOffset? from = null)
    {
        if (fromUnread && loadLastMessages)
        {
            throw new ValidationException("Невозможно одновременно получить и с непрочитанного сообщения и последение!");
        }

        if (!fromUnread && !loadLastMessages && from == null)
        {
            throw new ValidationException("При поиске не от непрочитанного сообщения надо указать параметр от какого времени ищем!");
        }

        ChatId = chatId;
        From = from;
        FromUnread = fromUnread;
        LoadLastMessages = loadLastMessages;
        IsDescending = isDescending;
    }
}
