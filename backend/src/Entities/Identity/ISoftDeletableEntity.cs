namespace FitHub.Common.Entities.Identity;

public interface ISoftDeletableEntity
{
    public DateTimeOffset? DeletedAt { get; }

    public bool IsDeleted { get; }
}
