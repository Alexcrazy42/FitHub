using FitHub.Common.Entities.Identity;

namespace FitHub.Common.Entities.Storage;

public interface IPendingRepository<TEntity, TEntityId>
    where TEntity : class, IEntity<TEntityId>
    where TEntityId : IIdentifier
{

}
