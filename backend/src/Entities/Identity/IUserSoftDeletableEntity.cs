namespace FitHub.Common.Entities.Identity;

public interface IUserSoftDeletableEntity<out TKey, out TEntity> : ISoftDeletableEntity
{
    TKey? DeletedById { get; }
    TEntity? DeletedBy { get; }
}
