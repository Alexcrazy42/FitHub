using FitHub.BankManager.Domain;
using FitHub.Common.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FitHub.BankManager.Data;

public class BankManagerDataContext : DbContext
{
    public BankManagerDataContext(DbContextOptions<BankManagerDataContext> options) : base(options)
    {
        ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();

    public DbSet<PaymentIntent> PaymentIntents => Set<PaymentIntent>();

    public DbSet<PaymentOperation> PaymentOperations => Set<PaymentOperation>();

    public DbSet<BankWebhookEvent> BankWebhookEvents => Set<BankWebhookEvent>();

    public DbSet<RabbitOutboxMessage> RabbitOutboxMessages => Set<RabbitOutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankManagerDataContext).Assembly);

        modelBuilder.UseCommonConventions();
    }

    /// <summary>
    /// Применить миграции
    /// </summary>
    public Task MigrateAsync()
        => Database.MigrateAsync();
}
