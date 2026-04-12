using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class ProductCategory : IEntity<ProductCategoryId>
{
    private ProductCategory(ProductCategoryId id, string name, string slug, ProductCategoryId? parentId)
    {
        Id = id;
        Name = name;
        Slug = slug;
        ParentId = parentId;
        IsActive = true;
    }

    public ProductCategoryId Id { get; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public ProductCategoryId? ParentId { get; private set; }
    public ProductCategory? Parent { get; private set; }
    public bool IsActive { get; private set; }

    public static ProductCategory Create(string name, string slug, ProductCategoryId? parentId = null)
    {
        return new ProductCategory(ProductCategoryId.New(), name, slug, parentId);
    }
}
