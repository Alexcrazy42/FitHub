using System.Linq.Expressions;
using System.Reflection;
using FitHub.Application.Marketplace;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Application.Marketplace;

public class MarketplaceCheckoutServiceTests
{
    [Fact(DisplayName = "Checkout reserves the last available item")]
    public async Task CreateReservationAsync_ShouldReserveLastAvailableItem()
    {
        var variant = CreateVariant(quantityOnHand: 1);
        var repository = new FakeStockReservationRepository
        {
            Variant = variant
        };
        var service = new MarketplaceCheckoutService(repository, new FakeUnitOfWork());

        var reservation = await service.CreateReservationAsync(
            new CreateCheckoutReservationCommand(variant.Id, 1, "cart-last-item"),
            CancellationToken.None);

        reservation.Status.ShouldBe(StockReservationStatus.Active);
        reservation.Quantity.ShouldBe(1);
        repository.AddedReservations.ShouldContain(reservation);
        variant.Inventory!.QuantityReserved.ShouldBe(1);
        variant.Inventory.AvailableQuantity.ShouldBe(0);
    }

    [Fact(DisplayName = "Checkout returns existing reservation for same idempotency key")]
    public async Task CreateReservationAsync_ShouldReturnExistingReservationForSameIdempotencyKey()
    {
        var variant = CreateVariant(quantityOnHand: 2);
        var existingReservation = StockReservation.Create(
            variant.Id,
            1,
            DateTimeOffset.UtcNow.AddMinutes(10),
            "same-key");
        var repository = new FakeStockReservationRepository
        {
            Variant = variant,
            ReservationByIdempotencyKey = existingReservation
        };
        var service = new MarketplaceCheckoutService(repository, new FakeUnitOfWork());

        var reservation = await service.CreateReservationAsync(
            new CreateCheckoutReservationCommand(variant.Id, 1, "same-key"),
            CancellationToken.None);

        reservation.ShouldBeSameAs(existingReservation);
        repository.AddedReservations.ShouldBeEmpty();
        variant.Inventory!.QuantityReserved.ShouldBe(0);
    }

    [Fact(DisplayName = "Checkout releases expired active reservations")]
    public async Task ReleaseExpiredReservationsAsync_ShouldReleaseInventoryAndExpireReservation()
    {
        var variant = CreateVariant(quantityOnHand: 1);
        variant.Inventory!.TryReserve(1).ShouldBeTrue();

        var reservation = StockReservation.Create(
            variant.Id,
            1,
            DateTimeOffset.UtcNow.AddMinutes(-1),
            "expired-key");
        SetPrivateProperty(reservation, nameof(StockReservation.ProductVariant), variant);

        var repository = new FakeStockReservationRepository
        {
            ExpiredReservations = [reservation]
        };
        var service = new MarketplaceCheckoutService(repository, new FakeUnitOfWork());

        await service.ReleaseExpiredReservationsAsync(DateTimeOffset.UtcNow, CancellationToken.None);

        reservation.Status.ShouldBe(StockReservationStatus.Expired);
        variant.Inventory.QuantityReserved.ShouldBe(0);
        variant.Inventory.AvailableQuantity.ShouldBe(1);
    }

    [Fact(DisplayName = "Checkout rejects unavailable quantity")]
    public async Task CreateReservationAsync_ShouldRejectUnavailableQuantity()
    {
        var variant = CreateVariant(quantityOnHand: 1);
        var repository = new FakeStockReservationRepository
        {
            Variant = variant
        };
        var service = new MarketplaceCheckoutService(repository, new FakeUnitOfWork());

        var exception = await Should.ThrowAsync<ValidationException>(() =>
            service.CreateReservationAsync(
                new CreateCheckoutReservationCommand(variant.Id, 2, "too-many"),
                CancellationToken.None));

        exception.Message.ShouldBe("Товар закончился или уже зарезервирован другим пользователем.");
        repository.AddedReservations.ShouldBeEmpty();
        variant.Inventory!.QuantityReserved.ShouldBe(0);
    }

    private static ProductVariant CreateVariant(int quantityOnHand)
    {
        var variant = ProductVariant.Create(ProductId.New(), $"SKU-{Guid.NewGuid():N}", 100m);
        var inventory = ProductVariantInventory.Create(variant.Id, quantityOnHand);
        SetPrivateProperty(variant, nameof(ProductVariant.Inventory), inventory);
        return variant;
    }

    private static void SetPrivateProperty<TObject, TValue>(TObject instance, string propertyName, TValue value)
    {
        typeof(TObject)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(instance, value);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }

    private sealed class FakeStockReservationRepository : IStockReservationRepository
    {
        public ProductVariant? Variant { get; init; }

        public StockReservation? ReservationByIdempotencyKey { get; init; }

        public IReadOnlyList<StockReservation> ExpiredReservations { get; init; } = [];

        public List<StockReservation> AddedReservations { get; } = [];

        public Task<StockReservation?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct)
        {
            return Task.FromResult(ReservationByIdempotencyKey);
        }

        public Task<StockReservation?> GetDetailsAsync(StockReservationId reservationId, CancellationToken ct)
        {
            return Task.FromResult<StockReservation?>(null);
        }

        public Task<ProductVariant?> GetVariantForReservationAsync(ProductVariantId productVariantId, CancellationToken ct)
        {
            return Task.FromResult(Variant?.Id == productVariantId ? Variant : null);
        }

        public Task<IReadOnlyList<StockReservation>> GetExpiredActiveReservationsAsync(DateTimeOffset now, CancellationToken ct)
        {
            return Task.FromResult(ExpiredReservations);
        }

        public Task<StockReservation?> GetFirstOrDefaultAsync(Expression<Func<StockReservation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<StockReservation?> GetSingleOrDefaultAsync(Expression<Func<StockReservation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<StockReservation>> GetAllAsync(Expression<Func<StockReservation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public void PendingUpdate(StockReservation entity)
        {
        }

        public void PendingUpdateRange(IReadOnlyCollection<StockReservation> entities)
        {
        }

        public Task PendingAddAsync(StockReservation entity, CancellationToken cancellationToken = default)
        {
            AddedReservations.Add(entity);
            return Task.CompletedTask;
        }

        public Task PendingAddRangeAsync(IReadOnlyCollection<StockReservation> entities, CancellationToken cancellationToken = default)
        {
            AddedReservations.AddRange(entities);
            return Task.CompletedTask;
        }

        public void PendingRemove(StockReservation entity)
        {
        }

        public void PendingRemoveRange(IReadOnlyList<StockReservation> entities)
        {
        }
    }
}
