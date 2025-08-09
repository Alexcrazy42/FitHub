using FitHub.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitHub.Data;

public sealed class DataContext : DbContext
{
    private readonly IReadOnlyList<IInterceptor> interceptors;

    public DataContext(
        DbContextOptions<DataContext> options,
        IEnumerable<IInterceptor> interceptors)
        : base(options)
    {
        this.interceptors = interceptors.ToList();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(interceptors);

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
