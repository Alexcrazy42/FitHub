using FitHub.Common.Entities.Identity;

namespace FitHub.Common.Entities;

/// <summary>
/// Доменная сущность
/// </summary>
public interface IEntity<out TEntityId>
    where TEntityId : IIdentifier
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    TEntityId Id { get; }
}
