using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class Product : IEntity<ProductId>
{
    private readonly List<ProductImage> images = [];
    private readonly List<ProductVariant> variants = [];

    private Product(
        ProductId id,
        ProductCategoryId categoryId,
        MarketplaceBrandId? brandId,
        string name,
        string slug,
        DateTimeOffset createdAt)
    {
        Id = id;
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Slug = slug;
        IsActive = true;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public ProductId Id { get; }
    public ProductCategoryId CategoryId { get; private set; }
    public ProductCategory? Category { get; private set; }
    public MarketplaceBrandId? BrandId { get; private set; }
    public MarketplaceBrand? Brand { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long Version { get; private set; }

    public IReadOnlyList<ProductImage> Images => images;
    public IReadOnlyList<ProductVariant> Variants => variants;

    public void SetDescription(string? description)
    {
        Description = description;
        Touch();
    }

    public void AddImage(ProductImage image)
    {
        images.Add(image);
        Touch();
    }

    public void AddVariant(ProductVariant variant)
    {
        variants.Add(variant);
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public static Product Create(ProductCategoryId categoryId, MarketplaceBrandId? brandId, string name, string slug)
    {
        return new Product(ProductId.New(), categoryId, brandId, name, slug, DateTimeOffset.UtcNow);
    }
}
