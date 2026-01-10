using FitHub.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitHub.Common.EntityFramework.Interceptors;

public class AuditableEntitiesInterceptor : SaveChangesInterceptor
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

        var entities = context
            .ChangeTracker
            .Entries<IAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Property(x => x.CreatedAt).CurrentValue = utcNow;
            }
            entity.Property(x => x.UpdatedAt).CurrentValue = utcNow;
        }
    }
}
