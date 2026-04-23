using FitHub.Common.Entities;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Domain.Marketplace;

public class DeliveryTests
{
    [Fact(DisplayName = "Delivery status changes are written to event history")]
    public void ChangeStatus_ShouldWriteEventHistory()
    {
        var delivery = Delivery.CreateForOrder(MarketplaceOrderId.New());

        delivery.ChangeStatus(DeliveryStatus.Assembling, "Заказ собирается.");

        delivery.Status.ShouldBe(DeliveryStatus.Assembling);
        delivery.Events.Count.ShouldBe(2);
        delivery.Events.Last().Status.ShouldBe(DeliveryStatus.Assembling);
        delivery.Events.Last().Message.ShouldBe("Заказ собирается.");
    }

    [Fact(DisplayName = "Delivery final status cannot be changed")]
    public void ChangeStatus_ShouldRejectChangeFromFinalStatus()
    {
        var delivery = Delivery.CreateForOrder(MarketplaceOrderId.New());

        delivery.ChangeStatus(DeliveryStatus.Delivered);

        Should.Throw<ValidationException>(() => delivery.ChangeStatus(DeliveryStatus.Failed));
    }
}
