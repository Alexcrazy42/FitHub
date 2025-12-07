using System.Linq.Expressions;
using FitHub.Common.Entities.Storage;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Common.EntityFramework;

public abstract class DefaultPendingNoIdRepository<TEntity, TContext> : IPendingNoIdRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext Context;

    protected readonly DbSet<TEntity> DbSet;

    public DefaultPendingNoIdRepository(TContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    protected virtual IQueryable<TEntity> ReadRaw() => DbSet.AsQueryable();

    public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => ReadRaw()
            .FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => ReadRaw()
            .SingleOrDefaultAsync(predicate, cancellationToken);

    public Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => ReadRaw()
            .Where(predicate)
            .ToReadOnlyListAsync(cancellationToken);

    public async Task PendingAddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task PendingAddRangeAsync(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return;
        }

        await DbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public void PendingRemove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void PendingRemoveRange(IReadOnlyList<TEntity> entities)
    {
        if (!entities.Any())
        {
            return;
        }

        DbSet.RemoveRange(entities);
    }

    public void PendingUpdate(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void PendingUpdateRange(IReadOnlyCollection<TEntity> entities)
    {
        if (!entities.Any())
        {
            return;
        }

        DbSet.UpdateRange(entities);
    }
}
