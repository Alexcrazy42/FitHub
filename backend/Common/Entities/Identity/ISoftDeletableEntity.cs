namespace FitHub.Common.Entities.Identity;

public interface ISoftDeletableEntity
{
    DateTimeOffset? DeletedAt { get; }
}
