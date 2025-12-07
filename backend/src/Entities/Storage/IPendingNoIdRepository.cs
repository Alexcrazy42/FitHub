using System.Linq.Expressions;

namespace FitHub.Common.Entities.Storage;


public interface IPendingNoIdRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Получить первый элемент или <c>default</c> для <see cref="TEntity"/>
    /// </summary>
    /// <remarks>В большинстве случаев требуется применять GetSingleOrDefaultAsync</remarks>
    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить единственный элемент или <c>default</c> для <see cref="TEntity"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">Если найденных элементов больше одного</exception>
    Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все объекты
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отложенно обновить сущность
    /// </summary>
    void PendingUpdate(TEntity entity);

    /// <summary>
    /// Отложенно обновить сущности
    /// </summary>
    void PendingUpdateRange(IReadOnlyCollection<TEntity> entities);

    /// <summary>
    /// Отложенно добавить сущность
    /// </summary>
    Task PendingAddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отложенно добавить сущности
    /// </summary>
    Task PendingAddRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отложенно удалить сущность
    /// </summary>
    void PendingRemove(TEntity entity);

    /// <summary>
    /// Отложенно удалить сущности
    /// </summary>
    void PendingRemoveRange(IReadOnlyList<TEntity> entities);
}
