using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IStockReservationRepository : IPendingRepository<StockReservation, StockReservationId>
{
    Task<StockReservation?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct);

    Task<StockReservation?> GetDetailsAsync(StockReservationId reservationId, CancellationToken ct);

    Task<ProductVariant?> GetVariantForReservationAsync(ProductVariantId productVariantId, CancellationToken ct);

    Task<IReadOnlyList<StockReservation>> GetExpiredActiveReservationsAsync(DateTimeOffset now, CancellationToken ct);
}
