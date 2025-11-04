using Microsoft.EntityFrameworkCore;

namespace TickerQ.Jobs.Data;

public sealed class TickerDataContext : DbContext
{
    public TickerDataContext(DbContextOptions<TickerDataContext> options)
        : base(options)
    {

    }

    public async Task MigrateAsync()
        => await Database.MigrateAsync();
}
