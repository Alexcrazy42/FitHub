using FitHub.Entities.Identity;

namespace FitHub.Entities.Storage;

public interface IPendingRepository<TEntity, TEntityId>
    where TEntity : class, IEntity<TEntityId>
    where TEntityId : IIdentifier
{

}
