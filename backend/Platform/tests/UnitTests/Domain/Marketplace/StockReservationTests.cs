using FitHub.Domain.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Domain.Marketplace;

public class StockReservationTests
{
    [Fact(DisplayName = "Active reservation can expire once")]
    public void Expire_ShouldMoveActiveReservationToExpired()
    {
        var reservation = StockReservation.Create(
            ProductVariantId.New(),
            quantity: 1,
            DateTimeOffset.UtcNow.AddMinutes(15),
            "cart-1-variant-1");

        reservation.Expire();
        reservation.Expire();

        reservation.Status.ShouldBe(StockReservationStatus.Expired);
    }

    [Fact(DisplayName = "Active reservation can release once")]
    public void Release_ShouldMoveActiveReservationToReleased()
    {
        var reservation = StockReservation.Create(
            ProductVariantId.New(),
            quantity: 1,
            DateTimeOffset.UtcNow.AddMinutes(15),
            "cart-1-variant-1");

        reservation.Release();
        reservation.Expire();

        reservation.Status.ShouldBe(StockReservationStatus.Released);
    }
}
