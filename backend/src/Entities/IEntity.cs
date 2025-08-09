using FitHub.Entities.Identity;

namespace FitHub.Entities;

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
