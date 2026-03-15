using FitHub.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitHub.Common.EntityFramework.Interceptors;

public class SoftDeletableEntitiesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateTrackedEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateTrackedEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateTrackedEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var deletedEntities = context
            .ChangeTracker
            .Entries<ISoftDeletableEntity>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entity in deletedEntities)
        {
            entity.State = EntityState.Modified;
            entity.Property(x => x.DeletedAt).CurrentValue = utcNow;
        }
    }
}
