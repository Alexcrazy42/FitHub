using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Shared.Messaging;

namespace FitHub.Domain.Messaging;

public class Message : IEntity<MessageId>, IUserAuditableEntity<IdentityUserId, User>, IUserSoftDeletableEntity<IdentityUserId, User>
{
    private Chat? chat;
    private List<MessageAttachment> attachments = [];
    private List<Message> repliedMessages = [];

    private Message(MessageId id, ChatId chatId, string messageText)
    {
        Id = id;
        ChatId = chatId;
        MessageText = messageText;
    }

    public MessageId Id { get; }

    public ChatId ChatId { get; private set; }

    public Chat Chat
    {
        get => UnexpectedException.ThrowIfNull(chat, "Чат неожиданно оказался Null");
        private set => chat = value;
    }

    public string MessageText { get; private set; }

    public MessageId? ReplyMessageId { get; private set; }

    public Message? ReplyMessage { get; private set; }

    public List<Message> RepliedMessages => repliedMessages;

    public MessageId? ForwardedMessageId { get; private set; }

    public Message? ForwardedMessage { get; private set; }

    public IReadOnlyList<MessageAttachment> Attachments => attachments;

    public void SetMessageText(string messageText)
    {
        MessageText = messageText;
    }

    public void SetReplyMessage(Message? replyMessage)
    {
        ReplyMessageId = replyMessage?.Id;
        ReplyMessage = replyMessage;
    }

    public static Message Create(Chat chat, string messageText, Message? replyMessage = null)
    {
        return new Message(MessageId.New(), chat.Id, messageText)
        {
            ReplyMessageId = replyMessage?.Id,
            ReplyMessage = replyMessage
        };
    }

    #region CommonFiels

    public DateTimeOffset CreatedAt { get; }
    public IdentityUserId CreatedById { get; } = null!;
    public User? CreatedBy { get; }
    public DateTimeOffset UpdatedAt { get; }
    public IdentityUserId UpdatedById { get; } = null!;
    public User? UpdatedBy { get; }

    public DateTimeOffset? DeletedAt { get; }
    public bool IsDeleted { get; }

    public IdentityUserId? DeletedById { get; }
    public User? DeletedBy { get; }

    #endregion
}
