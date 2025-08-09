namespace FitHub.Entities.Storage;

/// <summary>
/// UnitOfWork для отложенных множества отложенных изменений
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Сохранение отложенных изменений
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
