using FitHub.Application.Marketplace;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Marketplace;

public class MarketplacePaymentRepository : DefaultPendingRepository<MarketplacePayment, MarketplacePaymentId, DataContext>, IMarketplacePaymentRepository
{
    public MarketplacePaymentRepository(DataContext context) : base(context)
    {
    }

    public Task<MarketplacePayment?> GetByReservationIdAsync(StockReservationId reservationId, CancellationToken ct)
    {
        return ReadRaw()
            .Include(x => x.Reservation)
                .ThenInclude(x => x!.ProductVariant)
                    .ThenInclude(x => x!.Inventory)
            .Include(x => x.Reservation)
                .ThenInclude(x => x!.ProductVariant)
                    .ThenInclude(x => x!.Product)
                        .ThenInclude(x => x!.Brand)
            .Include(x => x.Reservation)
                .ThenInclude(x => x!.ProductVariant)
                    .ThenInclude(x => x!.Product)
                        .ThenInclude(x => x!.Images)
            .Include(x => x.Reservation)
                .ThenInclude(x => x!.ProductVariant)
                    .ThenInclude(x => x!.Attributes)
                        .ThenInclude(x => x.AttributeDefinition)
            .Include(x => x.Reservation)
                .ThenInclude(x => x!.ProductVariant)
                    .ThenInclude(x => x!.Attributes)
                        .ThenInclude(x => x.AttributeOption)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.ReservationId == reservationId, ct);
    }
}
