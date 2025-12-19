namespace FitHub.Common.Entities.Identity;

public interface IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; }
}
