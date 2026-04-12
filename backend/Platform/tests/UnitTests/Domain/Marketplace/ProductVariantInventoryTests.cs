using FitHub.Domain.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Domain.Marketplace;

public class ProductVariantInventoryTests
{
    [Fact(DisplayName = "Inventory reserves available quantity")]
    public void TryReserve_ShouldReserveAvailableQuantity()
    {
        var inventory = ProductVariantInventory.Create(ProductVariantId.New(), quantityOnHand: 3);

        var reserved = inventory.TryReserve(2);

        reserved.ShouldBeTrue();
        inventory.QuantityReserved.ShouldBe(2);
        inventory.AvailableQuantity.ShouldBe(1);
        inventory.Version.ShouldBe(1);
    }

    [Fact(DisplayName = "Inventory rejects unavailable quantity")]
    public void TryReserve_ShouldRejectUnavailableQuantity()
    {
        var inventory = ProductVariantInventory.Create(ProductVariantId.New(), quantityOnHand: 1);

        var reserved = inventory.TryReserve(2);

        reserved.ShouldBeFalse();
        inventory.QuantityReserved.ShouldBe(0);
        inventory.AvailableQuantity.ShouldBe(1);
        inventory.Version.ShouldBe(0);
    }

    [Fact(DisplayName = "Inventory releases reserved quantity")]
    public void TryReleaseReserved_ShouldReturnReservedQuantityToAvailable()
    {
        var inventory = ProductVariantInventory.Create(ProductVariantId.New(), quantityOnHand: 2);
        inventory.TryReserve(2).ShouldBeTrue();

        var released = inventory.TryReleaseReserved(1);

        released.ShouldBeTrue();
        inventory.QuantityReserved.ShouldBe(1);
        inventory.AvailableQuantity.ShouldBe(1);
        inventory.Version.ShouldBe(2);
    }
}
