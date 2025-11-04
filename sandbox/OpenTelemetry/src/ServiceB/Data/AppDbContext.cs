using Microsoft.EntityFrameworkCore;

namespace ServiceB.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WeatherLog> WeatherLogs => Set<WeatherLog>();
}

public class WeatherLog
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
    public DateTime LoggedAt { get; set; }
    public string Source { get; set; } = "kafka";
}