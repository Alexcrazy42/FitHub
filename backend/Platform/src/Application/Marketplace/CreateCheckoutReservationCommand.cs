using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public record CreateCheckoutReservationCommand(
    ProductVariantId ProductVariantId,
    int Quantity,
    string IdempotencyKey);
