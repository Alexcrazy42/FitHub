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
public class MarketplaceOrderController : ControllerBase
{
    private readonly IMarketplaceOrderService orderService;
    private readonly IAccessService accessService;
    private readonly ICurrentIdentityUserIdAccessor currentUserIdAccessor;

    public MarketplaceOrderController(
        IMarketplaceOrderService orderService,
        IAccessService accessService,
        ICurrentIdentityUserIdAccessor currentUserIdAccessor)
    {
        this.orderService = orderService;
        this.accessService = accessService;
        this.currentUserIdAccessor = currentUserIdAccessor;
    }

    [HttpGet(ApiRoutesV1.MarketplaceMyOrders)]
    public async Task<ListResponse<MarketplaceOrderResponse>> GetMyOrdersAsync([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.GymAdminOnly);

        var orders = await orderService.GetOrdersAsync(currentUserIdAccessor.GetCurrentUserId(), paged.ToQuery(), ct);
        return orders.ToListResponse(MarketplaceResponseExtensions.ToResponse);
    }

    [HttpGet(ApiRoutesV1.MarketplaceOrderById)]
    public async Task<MarketplaceOrderResponse> GetOrderAsync([FromRoute] string? id, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);

        var order = await orderService.GetOrderAsync(MarketplaceOrderId.Parse(id), ct);

        if (order is null)
        {
            throw new NotFoundException("Заказ не найден.");
        }

        return order.ToResponse();
    }
}
