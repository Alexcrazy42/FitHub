using FitHub.Application.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Application.Marketplace;

public class MarketplaceVariantAvailabilityTests
{
    [Fact(DisplayName = "Variant availability requires active variant, positive price and stock")]
    public void IsAvailable_ShouldRequireActiveVariantPositivePriceAndStock()
    {
        MarketplaceVariantAvailability.IsAvailable(true, 100m, 2, 1).ShouldBeTrue();
        MarketplaceVariantAvailability.IsAvailable(false, 100m, 2, 1).ShouldBeFalse();
        MarketplaceVariantAvailability.IsAvailable(true, 0m, 2, 1).ShouldBeFalse();
        MarketplaceVariantAvailability.IsAvailable(true, 100m, 1, 1).ShouldBeFalse();
        MarketplaceVariantAvailability.IsAvailable(true, 100m, null, null).ShouldBeFalse();
    }
}
