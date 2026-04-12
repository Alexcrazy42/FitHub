using FitHub.Application.Marketplace;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Marketplace;

public class StockReservationRepository : DefaultPendingRepository<StockReservation, StockReservationId, DataContext>, IStockReservationRepository
{
    public StockReservationRepository(DataContext context) : base(context)
    {
    }

    public Task<StockReservation?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct)
    {
        return ReadRaw()
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Inventory)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Product)
                    .ThenInclude(x => x!.Brand)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Product)
                    .ThenInclude(x => x!.Images)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Attributes)
                    .ThenInclude(x => x.AttributeDefinition)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Attributes)
                    .ThenInclude(x => x.AttributeOption)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
    }

    public Task<StockReservation?> GetDetailsAsync(StockReservationId reservationId, CancellationToken ct)
    {
        return ReadRaw()
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Inventory)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Product)
                    .ThenInclude(x => x!.Brand)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Product)
                    .ThenInclude(x => x!.Images)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Attributes)
                    .ThenInclude(x => x.AttributeDefinition)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Attributes)
                    .ThenInclude(x => x.AttributeOption)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == reservationId, ct);
    }

    public Task<ProductVariant?> GetVariantForReservationAsync(ProductVariantId productVariantId, CancellationToken ct)
    {
        return Context.Set<ProductVariant>()
            .Include(x => x.Inventory)
            .FirstOrDefaultAsync(x => x.Id == productVariantId, ct);
    }

    public async Task<IReadOnlyList<StockReservation>> GetExpiredActiveReservationsAsync(DateTimeOffset now, CancellationToken ct)
    {
        return await ReadRaw()
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x!.Inventory)
            .Where(x => x.Status == StockReservationStatus.Active && x.ExpiresAt <= now)
            .ToListAsync(ct);
    }
}
