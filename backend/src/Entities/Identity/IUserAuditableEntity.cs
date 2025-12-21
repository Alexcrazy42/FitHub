namespace FitHub.Common.Entities.Identity;

public interface IUserAuditableEntity<out TKey, out TEntity>
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
    TKey CreatedById { get; }
    TEntity? CreatedBy { get; }
    TKey UpdatedById { get; }
    TEntity? UpdatedBy { get; }
}
