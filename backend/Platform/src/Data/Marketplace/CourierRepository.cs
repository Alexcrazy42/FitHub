using FitHub.Application.Marketplace;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Marketplace;

public class CourierRepository : DefaultPendingRepository<Courier, CourierId, DataContext>, ICourierRepository
{
    private readonly DataContext context;

    public CourierRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public Task<Courier?> GetByIdAsync(CourierId courierId, CancellationToken ct)
    {
        return ReadRaw().FirstOrDefaultAsync(x => x.Id == courierId, ct);
    }

    public Task<Courier?> GetByNameAsync(string name, CancellationToken ct)
    {
        return ReadRaw().FirstOrDefaultAsync(x => x.Name == name, ct);
    }

    public async Task<IReadOnlyList<Courier>> GetAvailableForAssignmentAsync(int take, CancellationToken ct)
    {
        return await context.Couriers
            .FromSqlInterpolated($"""
                SELECT *
                FROM couriers
                WHERE is_available = TRUE
                ORDER BY created_at
                LIMIT {take}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(ct);
    }
}
