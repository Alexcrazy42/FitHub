using FitHub.Application.Marketplace;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Marketplace;

[ApiController]
public class MarketplaceCheckoutController : ControllerBase
{
    private readonly IMarketplaceCheckoutService checkoutService;
    private readonly IAccessService accessService;

    public MarketplaceCheckoutController(IMarketplaceCheckoutService checkoutService, IAccessService accessService)
    {
        this.checkoutService = checkoutService;
        this.accessService = accessService;
    }

    [HttpPost(ApiRoutesV1.MarketplaceCheckoutReservations)]
    public async Task<CheckoutReservationResponse> CreateReservationAsync(
        [FromBody] CreateCheckoutReservationRequest request,
        CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.GymAdminOnly);

        var reservation = await checkoutService.CreateReservationAsync(new CreateCheckoutReservationCommand(
            ProductVariantId.Parse(request.ProductVariantId),
            request.Quantity,
            request.IdempotencyKey), ct);

        return reservation.ToResponse();
    }

    [HttpGet(ApiRoutesV1.MarketplaceCheckoutReservationById)]
    public async Task<CheckoutReservationResponse> GetReservationAsync([FromRoute] string? id, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.GymAdminOnly);

        var reservationId = StockReservationId.Parse(id);
        var reservation = await checkoutService.GetReservationAsync(reservationId, ct);

        if (reservation is null)
        {
            throw new NotFoundException("Резерв не найден.");
        }

        return reservation.ToResponse();
    }
}
