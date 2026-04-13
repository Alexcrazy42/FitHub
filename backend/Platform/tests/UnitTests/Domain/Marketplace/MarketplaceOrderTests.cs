using System.Reflection;
using FitHub.Domain.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Domain.Marketplace;

public class MarketplaceOrderTests
{
    [Fact(DisplayName = "Paid reservation creates order with price and variant snapshot")]
    public void CreateFromPaidReservation_ShouldSnapshotOrderItem()
    {
        var product = Product.Create(ProductCategoryId.New(), null, "Training mat", "training-mat");
        var variant = ProductVariant.Create(product.Id, "MAT-BLACK", 1200m, name: "Black");
        SetPrivateProperty(variant, nameof(ProductVariant.Product), product);

        var reservation = StockReservation.Create(
            variant.Id,
            quantity: 2,
            DateTimeOffset.UtcNow.AddMinutes(15),
            "reservation-key");
        SetPrivateProperty(reservation, nameof(StockReservation.ProductVariant), variant);
        reservation.MarkPaid();

        var payment = MarketplacePayment.Create(reservation.Id, 2400m, "RUB", "payment-key");
        payment.ApplyBankStatus("bank-intent-1", "Paid", null);

        var order = MarketplaceOrder.CreateFromPaidReservation(reservation, payment);

        order.Status.ShouldBe(MarketplaceOrderStatus.Created);
        order.ReservationId.ShouldBe(reservation.Id);
        order.PaymentId.ShouldBe(payment.Id);
        order.TotalAmount.ShouldBe(2400m);
        order.Items.Count.ShouldBe(1);
        order.Items[0].ProductName.ShouldBe("Training mat");
        order.Items[0].Sku.ShouldBe("MAT-BLACK");
        order.Items[0].VariantName.ShouldBe("Black");
        order.Items[0].UnitPriceAmount.ShouldBe(1200m);
        order.Items[0].Quantity.ShouldBe(2);
        order.Items[0].TotalAmount.ShouldBe(2400m);
        order.StatusHistory.Count.ShouldBe(1);
    }

    private static void SetPrivateProperty<TObject, TValue>(TObject instance, string propertyName, TValue value)
    {
        typeof(TObject)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(instance, value);
    }
}
