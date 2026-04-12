using FitHub.Common.Entities;
using FitHub.Domain.Files;

namespace FitHub.Domain.Marketplace;

public class ProductImage : IEntity<ProductImageId>
{
    private ProductImage(ProductImageId id, ProductId productId, FileId fileId, int sortOrder, bool isMain)
    {
        Id = id;
        ProductId = productId;
        FileId = fileId;
        SortOrder = sortOrder;
        IsMain = isMain;
    }

    public ProductImageId Id { get; }
    public ProductId ProductId { get; private set; }
    public Product? Product { get; private set; }
    public FileId FileId { get; private set; }
    public FileEntity? File { get; private set; }
    public int SortOrder { get; private set; }
    public string? AltText { get; private set; }
    public bool IsMain { get; private set; }

    public static ProductImage Create(ProductId productId, FileId fileId, int sortOrder, bool isMain, string? altText = null)
    {
        return new ProductImage(ProductImageId.New(), productId, fileId, sortOrder, isMain)
        {
            AltText = altText
        };
    }
}
