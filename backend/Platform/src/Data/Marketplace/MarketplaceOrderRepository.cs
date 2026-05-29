using FitHub.Application.Common;
using FitHub.Application.Marketplace;
using FitHub.Authentication;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Marketplace;

public class MarketplaceOrderRepository : DefaultPendingRepository<MarketplaceOrder, MarketplaceOrderId, DataContext>, IMarketplaceOrderRepository
{
    public MarketplaceOrderRepository(DataContext context) : base(context)
    {
    }

    public Task<MarketplaceOrder?> GetByIdAsync(MarketplaceOrderId orderId, CancellationToken ct)
    {
        return WithDetails()
            .FirstOrDefaultAsync(x => x.Id == orderId, ct);
    }

    public Task<MarketplaceOrder?> GetByReservationIdAsync(StockReservationId reservationId, CancellationToken ct)
    {
        return WithDetails()
            .FirstOrDefaultAsync(x => x.ReservationId == reservationId, ct);
    }

    public Task<MarketplaceOrder?> GetByPaymentIdAsync(MarketplacePaymentId paymentId, CancellationToken ct)
    {
        return WithDetails()
            .FirstOrDefaultAsync(x => x.PaymentId == paymentId, ct);
    }

    public async Task<PagedResult<MarketplaceOrder>> GetByUserIdAsync(IdentityUserId userId, PagedQuery paged, CancellationToken ct)
    {
        var query = WithDetails()
            .Where(x => x.Reservation != null && x.Reservation.CreatedByUserId == userId)
            .OrderByDescending(x => x.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((paged.PageNumber - 1) * paged.PageSize)
            .Take(paged.PageSize)
            .ToListAsync(ct);

        return PagedResult<MarketplaceOrder>.Create(items, total, paged.PageNumber, paged.PageSize);
    }

    private IQueryable<MarketplaceOrder> WithDetails()
    {
        return ReadRaw()
            .Include(x => x.Reservation)
            .Include(x => x.Items)
            .Include(x => x.StatusHistory)
            .AsSplitQuery();
    }
}
