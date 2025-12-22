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

        if (!Participants.Any(x => x.UserId == userId))
        {
            return false;
        }

        return Participants.All(x => x.UserId != userId || !x.Blocked);
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

    public void SetParticipants(List<ChatParticipant> newParticipants, IdentityUserId userId)
    {
        if (Participants.Count != 0)
        {
            throw new UnexpectedException("Установить участников можно исключительно при создании чата");
        }

        if (Type == ChatType.OneToOne && newParticipants.Count != 2)
        {
            throw new ValidationException("В приватном чате должно быть два участника!");
        }

        if (Type == ChatType.Group && newParticipants.Count == 0)
        {
            throw new ValidationException("В группе должно быть несколько участников!");
        }

        if (!newParticipants.Select(x => x.UserId).Contains(userId))
        {
            throw new ValidationException("Нельзя создать чат без себя!");
        }

        participants = newParticipants;
    }

    public void SetGroupName(IdentityUserId currentUserId)
    {
        var groupName = GetGroupChatName(currentUserId);
        SetName(groupName);
    }

    public static Chat Create(ChatType type)
    {
        var chat = new Chat(ChatId.New(), type);

        return chat;
    }

    public string GetGroupChatName(IdentityUserId creatorId)
    {
        if (participants.Count == 0)
        {
            throw new ValidationException("Количество участников не может быть равным 0!");
        }

        const int maxNamesInTitle = 2;

        var sortedParticipants = participants
            .OrderByDescending(x => x.User.Id == creatorId) // true (1) идёт первым
            .ThenBy(x => x.User.Name)
            .ToList();

        var displayedNames = sortedParticipants
            .Take(maxNamesInTitle)
            .Select(x => x.User.Name)
            .ToList();

        var chatName = String.Join(", ", displayedNames);

        var remainingCount = participants.Count - maxNamesInTitle;
        if (remainingCount > 0)
        {
            chatName += $" + {remainingCount}";
        }

        return chatName;
    }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    public IdentityUserId CreatedById { get; } = null!;
    public User? CreatedBy { get; }
    public IdentityUserId UpdatedById { get; } = null!;
    public User? UpdatedBy { get; }
}
