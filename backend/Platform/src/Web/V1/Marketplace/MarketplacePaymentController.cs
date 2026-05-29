using FitHub.Application.Marketplace;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Marketplace;

[ApiController]
public class MarketplacePaymentController : ControllerBase
{
    private readonly IMarketplacePaymentService paymentService;
    private readonly IAccessService accessService;

    public MarketplacePaymentController(IMarketplacePaymentService paymentService, IAccessService accessService)
    {
        this.paymentService = paymentService;
        this.accessService = accessService;
    }

    [HttpPost(ApiRoutesV1.MarketplaceCheckoutReservationPaymentIntent)]
    public async Task<MarketplacePaymentIntentResponse> CreatePaymentIntentAsync([FromRoute] string? id, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.GymAdminOnly);
        var result = await paymentService.CreatePaymentIntentAsync(StockReservationId.Parse(id), ct);
        return result.ToResponse();
    }
}
