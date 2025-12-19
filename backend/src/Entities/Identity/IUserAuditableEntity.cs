namespace FitHub.Common.Entities.Identity;

public interface IUserAuditableEntity<out TKey, out TEntity> : IAuditableEntity
{
    TKey CreatedById { get; }
    TEntity? CreatedBy { get; }
    TKey UpdatedById { get; }
    TEntity? UpdatedBy { get; }
}
