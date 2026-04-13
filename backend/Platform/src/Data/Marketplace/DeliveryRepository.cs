using FitHub.Application.Common;
using FitHub.Application.Marketplace;
using FitHub.Authentication;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Marketplace;

public class DeliveryRepository : DefaultPendingRepository<Delivery, DeliveryId, DataContext>, IDeliveryRepository
{
    private readonly DataContext context;

    public DeliveryRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public Task<Delivery?> GetByIdAsync(DeliveryId deliveryId, CancellationToken ct)
    {
        return WithDetails().FirstOrDefaultAsync(x => x.Id == deliveryId, ct);
    }

    public Task<Delivery?> GetByOrderIdAsync(MarketplaceOrderId orderId, CancellationToken ct)
    {
        return WithDetails().FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
    }

    public Task<Delivery?> GetByOrderIdForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct)
    {
        return WithDetails()
            .FirstOrDefaultAsync(x =>
                x.OrderId == orderId &&
                x.Order != null &&
                x.Order.Reservation != null &&
                x.Order.Reservation.CreatedByUserId == userId,
                ct);
    }

    public async Task<PagedResult<Delivery>> GetAllAsync(PagedQuery paged, CancellationToken ct)
    {
        var query = WithDetails()
            .OrderByDescending(x => x.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((paged.PageNumber - 1) * paged.PageSize)
            .Take(paged.PageSize)
            .ToListAsync(ct);

        return PagedResult<Delivery>.Create(items, total, paged.PageNumber, paged.PageSize);
    }

    public async Task<IReadOnlyList<Delivery>> GetPendingForAssignmentAsync(int take, CancellationToken ct)
    {
        return await context.Deliveries
            .FromSqlInterpolated($"""
                SELECT *
                FROM deliveries
                WHERE courier_id IS NULL
                  AND status IN ('Pending', 'Assembling')
                ORDER BY created_at
                LIMIT {take}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Delivery>> GetExpiredCourierAssignmentsAsync(DateTimeOffset now, int take, CancellationToken ct)
    {
        return await context.Deliveries
            .FromSqlInterpolated($"""
                SELECT *
                FROM deliveries
                WHERE status = 'CourierAssigned'
                  AND courier_assignment_expires_at IS NOT NULL
                  AND courier_assignment_expires_at <= {now}
                ORDER BY courier_assignment_expires_at
                LIMIT {take}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(ct);
    }

    private IQueryable<Delivery> WithDetails()
    {
        return ReadRaw()
            .Include(x => x.Order)
                .ThenInclude(x => x!.Reservation)
            .Include(x => x.Order)
                .ThenInclude(x => x!.Items)
            .Include(x => x.Courier)
            .Include(x => x.Events)
            .Include(x => x.TrackingPoints)
            .AsSplitQuery();
    }
}
