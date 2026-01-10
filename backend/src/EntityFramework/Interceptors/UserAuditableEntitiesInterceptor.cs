using FitHub.Authentication;
using FitHub.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitHub.Common.EntityFramework.Interceptors;

public class UserAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;

    public UserAuditableEntitiesInterceptor(ICurrentIdentityUserIdAccessor userIdAccessor)
    {
        this.userIdAccessor = userIdAccessor;
    }

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

    private void UpdateTrackedEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entities = context
            .ChangeTracker
            .Entries<IUserAuditableEntity<IdentityUserId, User>>()
            .Where(e => e.State is EntityState.Modified or EntityState.Added)
            .ToList();

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entity in entities)
        {
            var currentUserId = userIdAccessor.GetCurrentUserId();

            if (entity.State == EntityState.Added)
            {
                // TODO: сделать type-safe
                entity.Property(nameof(IUserAuditableEntity<IdentityUserId, User>.CreatedAt)).CurrentValue = utcNow;
                entity.Property(nameof(IUserAuditableEntity<IdentityUserId, User>.CreatedById)).CurrentValue = currentUserId;
            }

            entity.Property(nameof(IUserAuditableEntity<IdentityUserId, User>.UpdatedAt)).CurrentValue = utcNow;
            entity.Property(nameof(IUserAuditableEntity<IdentityUserId, User>.UpdatedById)).CurrentValue = currentUserId;
        }
    }
}
