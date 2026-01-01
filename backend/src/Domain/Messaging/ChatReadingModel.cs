using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;


// TODO: отправили сообщение в чат - обновили у всех reading-model
// TODO: прочитал сообщения: обновил MessageView и ReadingModel

public class ChatReadingModel : IEntity<ChatReadingModelId>, IAuditableEntity
{
    private Chat? chat;
    private User? user;
    private Message? lastMessage;

    public ChatReadingModel(ChatReadingModelId id, ChatId chatId, IdentityUserId userId, MessageId lastMessageId, string lastMessageText, DateTimeOffset lastMessageTime, int unreadCount)
    {
        Id = id;
        ChatId = chatId;
        UserId = userId;
        UnreadCount = unreadCount;
        LastMessageTime = lastMessageTime;
        LastMessageId = lastMessageId;
        LastMessageText = lastMessageText;
    }

    public ChatReadingModelId Id { get; }

    public ChatId ChatId { get; private set; }

    public Chat Chat
    {
        get => UnexpectedException.ThrowIfNull(chat, "Чат неожиданно оказался null");
        private set => chat = value;
    }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользователь неожиданно оказался null");
        private set => user = value;
    }


    public MessageId LastMessageId { get; private set; }

    public Message LastMessage
    {
        get => UnexpectedException.ThrowIfNull(lastMessage, "Последнее сообщение неожиданно оказалось null");
        private set => lastMessage = value;
    }

    public string LastMessageText { get; private set; }

    public DateTimeOffset LastMessageTime { get; private set; }

    public int UnreadCount { get; private set; }

    #region CommonFields
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    #endregion
}
