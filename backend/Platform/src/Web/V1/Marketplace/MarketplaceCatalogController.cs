using FitHub.Application.Marketplace;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Marketplace;

[ApiController]
public class MarketplaceCatalogController : ControllerBase
{
    private readonly IMarketplaceCatalogService catalogService;
    private readonly IAccessService accessService;

    public MarketplaceCatalogController(IMarketplaceCatalogService catalogService, IAccessService accessService)
    {
        this.catalogService = catalogService;
        this.accessService = accessService;
    }

    [HttpPost(ApiRoutesV1.MarketplaceCatalogProductsSearch)]
    public async Task<MarketplaceCatalogSearchResponse> SearchProductsAsync(
        [FromBody] MarketplaceCatalogSearchRequest? request,
        CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        var result = await catalogService.SearchProductsAsync(request.ToCommand(), ct);

        return new MarketplaceCatalogSearchResponse(
            result.Products.Items.Select(MarketplaceResponseExtensions.ToListItemResponse).ToList(),
            result.Products.TotalItems ?? result.Products.Items.Count,
            result.Categories.Select(MarketplaceResponseExtensions.ToResponse).ToList(),
            result.Facets.Select(MarketplaceResponseExtensions.ToResponse).ToList());
    }

    [HttpGet(ApiRoutesV1.MarketplaceCatalogProductById)]
    public async Task<MarketplaceProductDetailsResponse> GetProductAsync([FromRoute] string? id, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);

        var productId = ProductId.Parse(id);
        var product = await catalogService.GetProductAsync(productId, ct);

        if (product is null)
        {
            throw new NotFoundException("Товар не найден");
        }

        return product.ToDetailsResponse();
    }
}
