using System.ComponentModel.DataAnnotations;

namespace ServiceA.Data;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WeatherRecord> WeatherRecords => Set<WeatherRecord>();
}

public class WeatherRecord
{
    [Key]
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
    public DateTime CreatedAt { get; set; }
}