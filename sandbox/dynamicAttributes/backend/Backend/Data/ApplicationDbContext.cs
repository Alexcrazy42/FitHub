using System.Text.Json;
using System.Text.Json.Nodes;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<CategoryHierarchy> CategoryHierarchies => Set<CategoryHierarchy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10,2)").IsRequired();
            
            // JSONB column for dynamic attributes
            entity.Property(e => e.Attributes)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions()),
                    new ValueComparer<Dictionary<string, object>>(
                        (c1, c2) => JsonSerializer.Serialize(c1) == JsonSerializer.Serialize(c2),
                        c => c?.Aggregate(0, (a, x) => HashCode.Combine(a, x.GetHashCode())) ?? 0,
                        c => c));
            
            // GIN index for JSONB
            entity.HasIndex(e => e.Attributes)
                .HasMethod("GIN");
            
            // B-tree index for price
            entity.HasIndex(e => e.Price);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            
            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(e => e.ProductCategories)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductCategory configuration (M:N)
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.CategoryId });
            
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Categories)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CategoryHierarchy configuration (Closure Table)
        modelBuilder.Entity<CategoryHierarchy>(entity =>
        {
            entity.HasKey(e => new { e.AncestorId, e.DescendantId });
            
            entity.HasOne(e => e.Ancestor)
                .WithMany()
                .HasForeignKey(e => e.AncestorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Descendant)
                .WithMany()
                .HasForeignKey(e => e.DescendantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.DescendantId);
            entity.HasIndex(e => e.AncestorId);
        });
    }
}
