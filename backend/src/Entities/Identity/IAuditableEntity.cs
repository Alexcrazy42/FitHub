namespace FitHub.Entities.Identity;

public interface IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get;  }

    public void SetCreatedAt(DateTimeOffset date);

    public void SetUpdatedAt(DateTimeOffset date);
}
