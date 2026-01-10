using FitHub.Authentication;
using FitHub.Common.Entities;

namespace FitHub.Domain.Messaging;

public class MessageView : IEntity<MessageViewId>
{
    private Message? message;
    private User? user;

    private MessageView(MessageViewId id, MessageId messageId, IdentityUserId userId)
    {
        Id = id;
        MessageId = messageId;
        UserId = userId;
    }

    public MessageViewId Id { get; }

    public MessageId MessageId { get; private set; }

    public Message Message
    {
        get => UnexpectedException.ThrowIfNull(message, "Сообщение неожиданно оказалось null");
        private set => message = value;
    }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользователь неожиданно оказался null");
        private set => user = value;
    }

    public DateTimeOffset? ViewedAt { get; private set; }

    public void SetViewedAt(DateTimeOffset viewedAt)
    {
        ViewedAt = viewedAt;
    }

    public static MessageView Create(Message message, User user)
    {
        return new MessageView(MessageViewId.New(), message.Id, user.Id);
    }
}
