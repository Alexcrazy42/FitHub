using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Domain.Users;

namespace FitHub.Domain.Messaging;

public class ChatEvent : IEntity<ChatEventId>, IUserAuditableEntity<IdentityUserId, User>, IUserSoftDeletableEntity<IdentityUserId, User>
{
    private Chat? chat;

    private ChatEvent(ChatEventId id, ChatId chatId, string? messageText)
    {
        Id = id;
        ChatId = chatId;
        MessageText = messageText;
    }

    public ChatEventId Id { get; }

    public ChatId ChatId { get; private set; }

    public Chat Chat
    {
        get => UnexpectedException.ThrowIfNull(chat, "Чат неожиданно оказался Null");
        private set => chat = value;
    }

    public string? MessageText { get; private set; }

    public static ChatEvent Create(Chat chat, string? messageText)
    {
        return new ChatEvent(ChatEventId.New(), chat.Id, messageText);
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
