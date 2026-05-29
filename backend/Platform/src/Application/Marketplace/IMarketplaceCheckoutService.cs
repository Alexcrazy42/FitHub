using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplaceCheckoutService
{
    Task<StockReservation> CreateReservationAsync(CreateCheckoutReservationCommand command, CancellationToken ct);

    Task<StockReservation?> GetReservationAsync(StockReservationId reservationId, CancellationToken ct);

    Task<int> ReleaseExpiredReservationsAsync(DateTimeOffset now, CancellationToken ct);
}
