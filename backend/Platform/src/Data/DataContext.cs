using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Messaging;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FitHub.Data;

public sealed class DataContext : DbContext
{
    public DataContext(
        DbContextOptions<DataContext> options)
        : base(options)
    {
        ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
    }

    public DbSet<BaseGroupTraining> BaseGroupTrainings => Set<BaseGroupTraining>();

    public DbSet<MessageView> MessageView => Set<MessageView>();

    public DbSet<MarketplaceBrand> MarketplaceBrands => Set<MarketplaceBrand>();

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    public DbSet<ProductVariantInventory> ProductVariantInventories => Set<ProductVariantInventory>();

    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();

    public DbSet<AttributeOption> AttributeOptions => Set<AttributeOption>();

    public DbSet<ProductVariantAttribute> ProductVariantAttributes => Set<ProductVariantAttribute>();

    public DbSet<StockReservation> StockReservations => Set<StockReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        modelBuilder.UseCommonConventions();
    }

    /// <summary>
    /// Применить миграции
    /// </summary>
    public Task MigrateAsync()
        => Database.MigrateAsync();
}
