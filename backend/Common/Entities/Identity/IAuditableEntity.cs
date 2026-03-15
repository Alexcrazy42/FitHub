namespace FitHub.Common.Entities.Identity;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; }

    DateTimeOffset UpdatedAt { get; }
}
