namespace FitHub.Common.Entities.Identity;

public interface IUserSoftDeletableEntity<out TKey, out TEntity>
{
    DateTimeOffset? DeletedAt { get; }
    TKey? DeletedById { get; }
    TEntity? DeletedBy { get; }
}
