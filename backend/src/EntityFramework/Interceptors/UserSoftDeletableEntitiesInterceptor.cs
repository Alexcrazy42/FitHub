using FitHub.Authentication;
using FitHub.Common.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitHub.Common.EntityFramework.Interceptors;

public class UserSoftDeletableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;

    public UserSoftDeletableEntitiesInterceptor(ICurrentIdentityUserIdAccessor userIdAccessor)
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

        var deletedEntities = context
            .ChangeTracker
            .Entries<IUserSoftDeletableEntity<IdentityUserId, User>>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entity in deletedEntities)
        {
            var currentUserId = userIdAccessor.GetCurrentUserId();
            entity.State = EntityState.Modified;
            // TODO: сделать type-safe
            entity.Property(nameof(IUserSoftDeletableEntity<IdentityUserId, User>.DeletedAt)).CurrentValue = utcNow;
            entity.Property(nameof(IUserSoftDeletableEntity<IdentityUserId, User>.DeletedById)).CurrentValue = currentUserId;
        }
    }
}
