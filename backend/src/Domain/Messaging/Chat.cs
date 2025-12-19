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

    /// <summary>
    /// Участники
    /// </summary>
    public IReadOnlyList<ChatParticipant> Participants => participants;


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
