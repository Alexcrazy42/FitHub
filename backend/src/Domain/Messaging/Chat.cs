using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Shared.Messaging;

namespace FitHub.Domain.Messaging;

public class Chat : IEntity<ChatId>, IUserAuditableEntity<IdentityUserId, User>
{
    private List<ChatParticipant> participants = [];

    private Chat(ChatId id, ChatType type)
    {
        Id = id;
        Type = type;
    }

    /// <summary>
    /// ИД
    /// </summary>
    public ChatId Id { get; }

    /// <summary>
    /// Тип чата
    /// </summary>
    public ChatType Type { get; private set; }

    public string? Name { get; private set; }

    /// <summary>
    /// Участники
    /// </summary>
    public IReadOnlyList<ChatParticipant> Participants => participants;

    public void SetName(string name)
    {
        if (Type == ChatType.OneToOne)
        {
            throw new ValidationException("Нельзя дать имя приватному чату!");
        }

        Name = name;
    }

    public bool HasAccess(IdentityUserId userId)
    {
        if (Participants.Count == 0)
        {
            throw new UnexpectedException("Нет участников!");
        }
        return Participants.All(x => x.UserId != userId || x.Blocked);
    }

    public void CheckAccess(IdentityUserId userId)
    {
        if (!HasAccess(userId))
        {
            throw new ValidationException("Нет доступа к чату!");
        }
    }

    public void AddParticipant(ChatParticipant participant)
    {
        if (Type != ChatType.Group)
        {
            throw new ValidationException("Добавить человека можно только в группу!");
        }

        if (participants.Count == 0)
        {
            throw new UnexpectedException("Нет участников!");
        }

        if (participants.Any(x => x.UserId == participant.UserId))
        {
            throw new ValidationException("Пользователь уже находится в чате!");
        }
        participants.Add(participant);
    }

    public static Chat Create(ChatType type)
    {
        return new Chat(ChatId.New(), type);
    }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    public IdentityUserId CreatedById { get; } = null!;
    public User? CreatedBy { get; }
    public IdentityUserId UpdatedById { get; } = null!;
    public User? UpdatedBy { get; }
}
