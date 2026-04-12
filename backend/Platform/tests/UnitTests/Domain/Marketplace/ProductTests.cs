using FitHub.Domain.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Domain.Marketplace;

public class ProductTests
{
    [Fact(DisplayName = "Product keeps created variant")]
    public void Product_ShouldKeepCreatedVariant()
    {
        var category = ProductCategory.Create("Accessories", "accessories");
        var brand = MarketplaceBrand.Create("FitHub", "fithub");
        var product = Product.Create(category.Id, brand.Id, "Training Mat", "training-mat");
        var variant = ProductVariant.Create(product.Id, "MAT-M", 2490m);

        product.AddVariant(variant);

        product.Variants.ShouldContain(variant);
        product.Version.ShouldBe(1);
    }
}
