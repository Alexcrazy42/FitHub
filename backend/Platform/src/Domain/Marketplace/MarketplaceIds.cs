using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Marketplace;

public class MarketplaceBrandId : GuidIdentifier<MarketplaceBrandId>, IIdentifierDescription
{
    public MarketplaceBrandId(Guid value) : base(value) { }

    public static string EntityTypeName => "Marketplace brand";
    public static string Prefix => FormatPrefix("fithub", "marketplace-brand");
}

public class ProductCategoryId : GuidIdentifier<ProductCategoryId>, IIdentifierDescription
{
    public ProductCategoryId(Guid value) : base(value) { }

    public static string EntityTypeName => "Product category";
    public static string Prefix => FormatPrefix("fithub", "product-category");
}

public class ProductId : GuidIdentifier<ProductId>, IIdentifierDescription
{
    public ProductId(Guid value) : base(value) { }

    public static string EntityTypeName => "Product";
    public static string Prefix => FormatPrefix("fithub", "product");
}

public class ProductImageId : GuidIdentifier<ProductImageId>, IIdentifierDescription
{
    public ProductImageId(Guid value) : base(value) { }

    public static string EntityTypeName => "Product image";
    public static string Prefix => FormatPrefix("fithub", "product-image");
}

public class ProductVariantId : GuidIdentifier<ProductVariantId>, IIdentifierDescription
{
    public ProductVariantId(Guid value) : base(value) { }

    public static string EntityTypeName => "Product variant";
    public static string Prefix => FormatPrefix("fithub", "product-variant");
}

public class ProductVariantInventoryId : GuidIdentifier<ProductVariantInventoryId>, IIdentifierDescription
{
    public ProductVariantInventoryId(Guid value) : base(value) { }

    public static string EntityTypeName => "Product variant inventory";
    public static string Prefix => FormatPrefix("fithub", "product-variant-inventory");
}

public class AttributeDefinitionId : GuidIdentifier<AttributeDefinitionId>, IIdentifierDescription
{
    public AttributeDefinitionId(Guid value) : base(value) { }

    public static string EntityTypeName => "Attribute definition";
    public static string Prefix => FormatPrefix("fithub", "attribute-definition");
}

public class AttributeOptionId : GuidIdentifier<AttributeOptionId>, IIdentifierDescription
{
    public AttributeOptionId(Guid value) : base(value) { }

    public static string EntityTypeName => "Attribute option";
    public static string Prefix => FormatPrefix("fithub", "attribute-option");
}

public class ProductVariantAttributeId : GuidIdentifier<ProductVariantAttributeId>, IIdentifierDescription
{
    public ProductVariantAttributeId(Guid value) : base(value) { }

    public static string EntityTypeName => "Product variant attribute";
    public static string Prefix => FormatPrefix("fithub", "product-variant-attribute");
}

public class StockReservationId : GuidIdentifier<StockReservationId>, IIdentifierDescription
{
    public StockReservationId(Guid value) : base(value) { }

    public static string EntityTypeName => "Stock reservation";
    public static string Prefix => FormatPrefix("fithub", "stock-reservation");
}
