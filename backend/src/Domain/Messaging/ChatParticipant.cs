using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Domain.Users;

namespace FitHub.Domain.Messaging;

/// <summary>
/// Участник чата
/// </summary>
public class ChatParticipant : IEntity<ChatParticipantId>
{
    private User? user;
    private Chat? chat;

    private ChatParticipant(ChatParticipantId id, IdentityUserId userId, ChatId chatId, DateTimeOffset joinedAt)
    {
        Id = id;
        UserId = userId;
        ChatId = chatId;
        JoinedAt = joinedAt;
    }

    public ChatParticipantId Id { get; }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользователь неожиданного оказался null");
        private set => user = value;
    }

    public ChatId ChatId { get; private set; }

    public Chat Chat
    {
        get => UnexpectedException.ThrowIfNull(chat, "Чат неожиданного оказался null");
        private set => chat = value;
    }

    public DateTimeOffset JoinedAt { get; private set; }

    public bool IsLeft { get; private set; }

    public static ChatParticipant Create(User user, Chat chat)
    {
        return new ChatParticipant(ChatParticipantId.New(), user.Id, chat.Id, DateTimeOffset.Now)
        {
            User = user,
            Chat = chat,
            IsLeft = false
        };
    }
}
