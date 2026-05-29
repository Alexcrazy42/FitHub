using FitHub.Application.Marketplace;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;

namespace FitHub.Web.V1.Marketplace;

public static class MarketplaceResponseExtensions
{
    public static MarketplaceCatalogSearchCommand ToCommand(this MarketplaceCatalogSearchRequest request)
    {
        return MarketplaceCatalogSearchCommand.Create(
            request.CategoryId,
            request.SearchText,
            request.MinPrice,
            request.MaxPrice,
            request.InStock,
            request.Facets,
            request.Sort,
            request.PageNumber,
            request.PageSize);
    }

    public static MarketplaceProductListItemResponse ToListItemResponse(Product product)
    {
        var activeVariants = product.Variants
            .Where(x => x.IsActive)
            .ToList();
        var cheapestVariant = activeVariants
            .OrderBy(x => x.PriceAmount)
            .FirstOrDefault();
        var mainImage = product.Images
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.SortOrder)
            .FirstOrDefault();

        return new MarketplaceProductListItemResponse(
            product.Id.ToString(),
            product.Name,
            product.Slug,
            product.Brand?.Name,
            product.CategoryId.ToString(),
            new MarketplaceMoneyResponse(cheapestVariant?.PriceAmount ?? 0m, cheapestVariant?.Currency ?? "RUB"),
            activeVariants.Any(x =>
                x.Inventory is not null &&
                x.Inventory.QuantityOnHand > x.Inventory.QuantityReserved),
            mainImage?.ToResponse());
    }

    public static MarketplaceProductDetailsResponse ToDetailsResponse(this Product product)
    {
        return new MarketplaceProductDetailsResponse(
            product.Id.ToString(),
            product.Name,
            product.Slug,
            product.Description,
            product.Brand?.Name,
            product.CategoryId.ToString(),
            product.Images
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.SortOrder)
                .Select(x => x.ToResponse())
                .ToList(),
            product.Variants
                .OrderBy(x => x.PriceAmount)
                .ThenBy(x => x.Sku)
                .Select(ToResponse)
                .ToList());
    }

    private static MarketplaceProductVariantResponse ToResponse(ProductVariant variant)
    {
        return new MarketplaceProductVariantResponse(
            variant.Id.ToString(),
            variant.Sku,
            variant.Name,
            new MarketplaceMoneyResponse(variant.PriceAmount, variant.Currency),
            variant.CompareAtPriceAmount is null
                ? null
                : new MarketplaceMoneyResponse(variant.CompareAtPriceAmount.Value, variant.Currency),
            variant.IsActive,
            MarketplaceVariantAvailability.IsAvailable(variant),
            variant.Inventory is null
                ? null
                : new MarketplaceStockResponse(
                    variant.Inventory.QuantityOnHand,
                    variant.Inventory.QuantityReserved,
                    variant.Inventory.AvailableQuantity),
            variant.Attributes
                .OrderBy(x => x.AttributeDefinition?.SortOrder ?? 0)
                .ThenBy(x => x.AttributeDefinition?.Name)
                .Select(ToResponse)
                .ToList());
    }

    private static MarketplaceVariantAttributeResponse ToResponse(ProductVariantAttribute attribute)
    {
        return new MarketplaceVariantAttributeResponse(
            attribute.AttributeDefinitionId.ToString(),
            attribute.AttributeOptionId.ToString(),
            attribute.AttributeDefinition?.Code ?? String.Empty,
            attribute.AttributeDefinition?.Name ?? String.Empty,
            attribute.AttributeOption?.Value ?? String.Empty);
    }

    public static MarketplaceFacetResponse ToResponse(MarketplaceCatalogFacet facet)
    {
        return new MarketplaceFacetResponse(
            facet.AttributeDefinitionId.ToString(),
            facet.Code,
            facet.Name,
            facet.IsPurchaseOption,
            facet.Values.Select(ToResponse).ToList());
    }

    public static MarketplaceCategoryFacetValueResponse ToResponse(MarketplaceCatalogCategoryFacetValue category)
    {
        return new MarketplaceCategoryFacetValueResponse(
            category.CategoryId.ToString(),
            category.Name,
            category.Slug,
            category.Count,
            category.Selected);
    }

    private static MarketplaceFacetValueResponse ToResponse(MarketplaceCatalogFacetValue value)
    {
        return new MarketplaceFacetValueResponse(
            value.AttributeOptionId.ToString(),
            value.Value,
            value.Count,
            value.Selected);
    }

    public static CheckoutReservationResponse ToResponse(this StockReservation reservation)
    {
        return new CheckoutReservationResponse(
            reservation.Id.ToString(),
            reservation.ProductVariantId.ToString(),
            reservation.Quantity,
            reservation.Status.ToString(),
            reservation.ExpiresAt,
            reservation.ProductVariant?.ToCheckoutItemResponse());
    }

    public static MarketplacePaymentIntentResponse ToResponse(this MarketplacePaymentResult result)
    {
        return new MarketplacePaymentIntentResponse(
            result.Reservation.ToResponse(),
            result.PaymentIntentId,
            result.PaymentStatus,
            new MarketplaceMoneyResponse(result.Amount, result.Currency),
            result.FailureReason,
            result.Order?.ToResponse());
    }

    public static MarketplaceOrderResponse ToResponse(this MarketplaceOrder order)
    {
        return new MarketplaceOrderResponse(
            order.Id.ToString(),
            order.ReservationId.ToString(),
            order.PaymentId.ToString(),
            order.Status.ToString(),
            new MarketplaceMoneyResponse(order.TotalAmount, order.Currency),
            order.CreatedAt,
            order.Items.Select(ToResponse).ToList(),
            order.StatusHistory
                .OrderBy(x => x.CreatedAt)
                .Select(ToResponse)
                .ToList());
    }

    private static MarketplaceOrderItemResponse ToResponse(MarketplaceOrderItem item)
    {
        return new MarketplaceOrderItemResponse(
            item.Id.ToString(),
            item.ProductId.ToString(),
            item.ProductVariantId.ToString(),
            item.ProductName,
            item.BrandName,
            item.Sku,
            item.VariantName,
            new MarketplaceMoneyResponse(item.UnitPriceAmount, item.Currency),
            item.Quantity,
            new MarketplaceMoneyResponse(item.TotalAmount, item.Currency),
            item.ImageFileId is null
                ? null
                : new MarketplaceProductImageResponse(
                    item.ImageFileId,
                    $"/api/v1/files/{item.ImageFileId}",
                    item.ProductName,
                    0,
                    true),
            item.AttributeSummary);
    }

    private static MarketplaceOrderStatusHistoryResponse ToResponse(MarketplaceOrderStatusHistory history)
    {
        return new MarketplaceOrderStatusHistoryResponse(
            history.Status.ToString(),
            history.CreatedAt,
            history.Reason);
    }

    public static DeliveryResponse ToResponse(this Delivery delivery)
    {
        return new DeliveryResponse(
            delivery.Id.ToString(),
            delivery.OrderId.ToString(),
            delivery.Status.ToString(),
            delivery.CourierId?.ToString(),
            delivery.Courier?.Name,
            delivery.PickupAddress,
            delivery.DropoffAddress,
            delivery.CreatedAt,
            delivery.LastStateChangedAt,
            delivery.CourierAssignmentExpiresAt,
            delivery.Events
                .OrderBy(x => x.CreatedAt)
                .Select(ToResponse)
                .ToList(),
            delivery.TrackingPoints
                .OrderBy(x => x.CreatedAt)
                .Select(ToResponse)
                .ToList(),
            delivery.Order?.ToResponse());
    }

    private static DeliveryEventResponse ToResponse(DeliveryEvent deliveryEvent)
    {
        return new DeliveryEventResponse(
            deliveryEvent.Status.ToString(),
            deliveryEvent.CreatedAt,
            deliveryEvent.Message);
    }

    private static DeliveryTrackingPointResponse ToResponse(DeliveryTrackingPoint point)
    {
        return new DeliveryTrackingPointResponse(
            point.Latitude,
            point.Longitude,
            point.CreatedAt);
    }

    private static CheckoutReservationItemResponse? ToCheckoutItemResponse(this ProductVariant variant)
    {
        if (variant.Product is null)
        {
            return null;
        }

        var image = variant.Product.Images
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.SortOrder)
            .FirstOrDefault();

        return new CheckoutReservationItemResponse(
            variant.Product.Id.ToString(),
            variant.Product.Name,
            variant.Product.Brand?.Name,
            variant.Sku,
            variant.Name,
            new MarketplaceMoneyResponse(variant.PriceAmount, variant.Currency),
            image?.ToResponse(),
            variant.Attributes
                .OrderBy(x => x.AttributeDefinition?.SortOrder ?? 0)
                .ThenBy(x => x.AttributeDefinition?.Name)
                .Select(ToResponse)
                .ToList());
    }

    private static MarketplaceProductImageResponse ToResponse(this ProductImage image)
    {
        return new MarketplaceProductImageResponse(
            image.FileId.ToString(),
            $"/api/v1/files/{image.FileId}",
            image.AltText,
            image.SortOrder,
            image.IsMain);
    }
}
