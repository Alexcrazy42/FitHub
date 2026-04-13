using FitHub.Application.Marketplace;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Domain.Marketplace;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Marketplace;

[ApiController]
public class MarketplaceDeliveryController : ControllerBase
{
    private readonly IDeliveryService deliveryService;
    private readonly IAccessService accessService;
    private readonly ICurrentIdentityUserIdAccessor currentUserIdAccessor;

    public MarketplaceDeliveryController(
        IDeliveryService deliveryService,
        IAccessService accessService,
        ICurrentIdentityUserIdAccessor currentUserIdAccessor)
    {
        this.deliveryService = deliveryService;
        this.accessService = accessService;
        this.currentUserIdAccessor = currentUserIdAccessor;
    }

    [HttpGet(ApiRoutesV1.MarketplaceDeliveries)]
    public async Task<ListResponse<DeliveryResponse>> GetDeliveriesAsync([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly);

        var deliveries = await deliveryService.GetDeliveriesAsync(paged.ToQuery(), ct);
        return deliveries.ToListResponse(MarketplaceResponseExtensions.ToResponse);
    }

    [HttpGet(ApiRoutesV1.MarketplaceDeliveryById)]
    public async Task<DeliveryResponse> GetDeliveryAsync([FromRoute] string? id, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly);

        var delivery = await deliveryService.GetDeliveryAsync(DeliveryId.Parse(id), ct);

        if (delivery is null)
        {
            throw new NotFoundException("Доставка не найдена.");
        }

        return delivery.ToResponse();
    }

    [HttpGet(ApiRoutesV1.MarketplaceOrderDelivery)]
    public async Task<DeliveryResponse> GetOrderDeliveryAsync([FromRoute] string? orderId, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.GymAdminOnly);

        var delivery = await deliveryService.GetDeliveryByOrderForUserAsync(
            MarketplaceOrderId.Parse(orderId),
            currentUserIdAccessor.GetCurrentUserId(),
            ct);

        if (delivery is null)
        {
            throw new NotFoundException("Доставка для заказа еще не создана.");
        }

        return delivery.ToResponse();
    }
}
