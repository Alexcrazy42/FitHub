using System.Linq.Expressions;
using System.Reflection;
using FitHub.Application.Common;
using FitHub.Application.Marketplace;
using FitHub.Application.Marketplace.Deliveries;
using FitHub.Application.Outbox;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Marketplace.Deliveries;
using FitHub.Domain.Outbox;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Application.Marketplace;

public class MarketplacePaymentServiceTests
{
    [Fact(DisplayName = "Paid payment creates one order from reservation")]
    public async Task ApplyBankPaymentStatusAsync_ShouldCreateOrderFromPaidReservation()
    {
        var variant = CreateVariant();
        var reservation = StockReservation.Create(variant.Id, 1, DateTimeOffset.UtcNow.AddMinutes(15), "reservation-key");
        SetPrivateProperty(reservation, nameof(StockReservation.ProductVariant), variant);
        var payment = MarketplacePayment.Create(reservation.Id, 1200m, "RUB", "payment-key");
        var orderRepository = new FakeMarketplaceOrderRepository();
        var deliveryRepository = new FakeDeliveryRepository();
        var service = CreateService(reservation, payment, orderRepository, deliveryRepository);

        await service.ApplyBankPaymentStatusAsync(
            reservation.Id,
            "bank-intent-1",
            "Paid",
            1200m,
            "RUB",
            null,
            CancellationToken.None);

        reservation.Status.ShouldBe(StockReservationStatus.Paid);
        payment.Status.ShouldBe(MarketplacePaymentStatus.Paid);
        orderRepository.AddedOrders.Count.ShouldBe(1);
        orderRepository.AddedOrders[0].ReservationId.ShouldBe(reservation.Id);
        orderRepository.AddedOrders[0].PaymentId.ShouldBe(payment.Id);
        deliveryRepository.AddedDeliveries.Count.ShouldBe(1);
        deliveryRepository.AddedDeliveries[0].OrderId.ShouldBe(orderRepository.AddedOrders[0].Id);
        deliveryRepository.AddedDeliveries[0].Status.ShouldBe(DeliveryStatus.Pending);
    }

    [Fact(DisplayName = "Duplicate paid payment handling does not create second order")]
    public async Task ApplyBankPaymentStatusAsync_ShouldReuseExistingOrder()
    {
        var variant = CreateVariant();
        var reservation = StockReservation.Create(variant.Id, 1, DateTimeOffset.UtcNow.AddMinutes(15), "reservation-key");
        SetPrivateProperty(reservation, nameof(StockReservation.ProductVariant), variant);
        reservation.MarkPaid();
        var payment = MarketplacePayment.Create(reservation.Id, 1200m, "RUB", "payment-key");
        payment.ApplyBankStatus("bank-intent-1", "Paid", null);
        var existingOrder = MarketplaceOrder.CreateFromPaidReservation(reservation, payment);
        var orderRepository = new FakeMarketplaceOrderRepository
        {
            OrderByPaymentId = existingOrder
        };
        var deliveryRepository = new FakeDeliveryRepository
        {
            DeliveryByOrderId = Delivery.CreateForOrder(existingOrder.Id)
        };
        var service = CreateService(reservation, payment, orderRepository, deliveryRepository);

        await service.ApplyBankPaymentStatusAsync(
            reservation.Id,
            "bank-intent-1",
            "Paid",
            1200m,
            "RUB",
            null,
            CancellationToken.None);

        orderRepository.AddedOrders.ShouldBeEmpty();
        deliveryRepository.AddedDeliveries.ShouldBeEmpty();
    }

    private static MarketplacePaymentService CreateService(
        StockReservation reservation,
        MarketplacePayment payment,
        FakeMarketplaceOrderRepository orderRepository,
        FakeDeliveryRepository deliveryRepository)
    {
        return new MarketplacePaymentService(
            new FakeStockReservationRepository { Reservation = reservation },
            new FakeMarketplacePaymentRepository { Payment = payment },
            orderRepository,
            deliveryRepository,
            new FakeOutboxRepository(),
            new FakeUnitOfWork());
    }

    private static ProductVariant CreateVariant()
    {
        var product = Product.Create(ProductCategoryId.New(), null, "Training mat", "training-mat");
        var variant = ProductVariant.Create(product.Id, "MAT-BLACK", 1200m, name: "Black");
        SetPrivateProperty(variant, nameof(ProductVariant.Product), product);
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

    private sealed class FakeOutboxRepository : IOutboxRepository
    {
        public Task AddAsync(RabbitOutboxMessage message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<RabbitOutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct)
        {
            return Task.FromResult<IReadOnlyList<RabbitOutboxMessage>>([]);
        }
    }

    private sealed class FakeStockReservationRepository : IStockReservationRepository
    {
        public StockReservation? Reservation { get; init; }

        public Task<StockReservation?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct)
        {
            return Task.FromResult<StockReservation?>(null);
        }

        public Task<StockReservation?> GetDetailsAsync(StockReservationId reservationId, CancellationToken ct)
        {
            return Task.FromResult(Reservation?.Id == reservationId ? Reservation : null);
        }

        public Task<ProductVariant?> GetVariantForReservationAsync(ProductVariantId productVariantId, CancellationToken ct)
        {
            return Task.FromResult<ProductVariant?>(null);
        }

        public Task<IReadOnlyList<StockReservation>> GetExpiredActiveReservationsAsync(DateTimeOffset now, CancellationToken ct)
        {
            return Task.FromResult<IReadOnlyList<StockReservation>>([]);
        }

        public Task<StockReservation?> GetFirstOrDefaultAsync(Expression<Func<StockReservation, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<StockReservation?> GetSingleOrDefaultAsync(Expression<Func<StockReservation, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<StockReservation>> GetAllAsync(Expression<Func<StockReservation, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void PendingUpdate(StockReservation entity) { }
        public void PendingUpdateRange(IReadOnlyCollection<StockReservation> entities) { }
        public Task PendingAddAsync(StockReservation entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PendingAddRangeAsync(IReadOnlyCollection<StockReservation> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void PendingRemove(StockReservation entity) { }
        public void PendingRemoveRange(IReadOnlyList<StockReservation> entities) { }
    }

    private sealed class FakeMarketplacePaymentRepository : IMarketplacePaymentRepository
    {
        public MarketplacePayment? Payment { get; init; }

        public Task<MarketplacePayment?> GetByReservationIdAsync(StockReservationId reservationId, CancellationToken ct)
        {
            return Task.FromResult(Payment?.ReservationId == reservationId ? Payment : null);
        }

        public Task<MarketplacePayment?> GetFirstOrDefaultAsync(Expression<Func<MarketplacePayment, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<MarketplacePayment?> GetSingleOrDefaultAsync(Expression<Func<MarketplacePayment, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<MarketplacePayment>> GetAllAsync(Expression<Func<MarketplacePayment, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void PendingUpdate(MarketplacePayment entity) { }
        public void PendingUpdateRange(IReadOnlyCollection<MarketplacePayment> entities) { }
        public Task PendingAddAsync(MarketplacePayment entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PendingAddRangeAsync(IReadOnlyCollection<MarketplacePayment> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void PendingRemove(MarketplacePayment entity) { }
        public void PendingRemoveRange(IReadOnlyList<MarketplacePayment> entities) { }
    }

    private sealed class FakeMarketplaceOrderRepository : IMarketplaceOrderRepository
    {
        public MarketplaceOrder? OrderByPaymentId { get; init; }
        public MarketplaceOrder? OrderByReservationId { get; init; }
        public List<MarketplaceOrder> AddedOrders { get; } = [];

        public Task<MarketplaceOrder?> GetByIdAsync(MarketplaceOrderId orderId, CancellationToken ct)
        {
            return Task.FromResult<MarketplaceOrder?>(null);
        }

        public Task<MarketplaceOrder?> GetByReservationIdAsync(StockReservationId reservationId, CancellationToken ct)
        {
            return Task.FromResult(OrderByReservationId?.ReservationId == reservationId ? OrderByReservationId : null);
        }

        public Task<MarketplaceOrder?> GetByPaymentIdAsync(MarketplacePaymentId paymentId, CancellationToken ct)
        {
            return Task.FromResult(OrderByPaymentId?.PaymentId == paymentId ? OrderByPaymentId : null);
        }

        public Task<PagedResult<MarketplaceOrder>> GetByUserIdAsync(IdentityUserId userId, PagedQuery paged, CancellationToken ct)
        {
            return Task.FromResult(PagedResult<MarketplaceOrder>.Create([]));
        }

        public Task<MarketplaceOrder?> GetFirstOrDefaultAsync(Expression<Func<MarketplaceOrder, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<MarketplaceOrder?> GetSingleOrDefaultAsync(Expression<Func<MarketplaceOrder, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<MarketplaceOrder>> GetAllAsync(Expression<Func<MarketplaceOrder, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void PendingUpdate(MarketplaceOrder entity) { }
        public void PendingUpdateRange(IReadOnlyCollection<MarketplaceOrder> entities) { }
        public Task PendingAddAsync(MarketplaceOrder entity, CancellationToken cancellationToken = default)
        {
            AddedOrders.Add(entity);
            return Task.CompletedTask;
        }
        public Task PendingAddRangeAsync(IReadOnlyCollection<MarketplaceOrder> entities, CancellationToken cancellationToken = default)
        {
            AddedOrders.AddRange(entities);
            return Task.CompletedTask;
        }
        public void PendingRemove(MarketplaceOrder entity) { }
        public void PendingRemoveRange(IReadOnlyList<MarketplaceOrder> entities) { }
    }

    private sealed class FakeDeliveryRepository : IDeliveryRepository
    {
        public Delivery? DeliveryByOrderId { get; init; }
        public List<Delivery> AddedDeliveries { get; } = [];

        public Task<Delivery?> GetByIdAsync(DeliveryId deliveryId, CancellationToken ct)
        {
            return Task.FromResult<Delivery?>(null);
        }

        public Task<Delivery?> GetByOrderIdAsync(MarketplaceOrderId orderId, CancellationToken ct)
        {
            return Task.FromResult(DeliveryByOrderId?.OrderId == orderId ? DeliveryByOrderId : null);
        }

        public Task<Delivery?> GetByOrderIdForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct)
        {
            return Task.FromResult<Delivery?>(null);
        }

        public Task<PagedResult<Delivery>> GetAllAsync(PagedQuery paged, CancellationToken ct)
        {
            return Task.FromResult(PagedResult<Delivery>.Create([]));
        }

        public Task<IReadOnlyList<Delivery>> GetPendingForAssignmentAsync(int take, CancellationToken ct)
        {
            return Task.FromResult<IReadOnlyList<Delivery>>([]);
        }

        public Task<IReadOnlyList<Delivery>> GetExpiredCourierAssignmentsAsync(DateTimeOffset now, int take, CancellationToken ct)
        {
            return Task.FromResult<IReadOnlyList<Delivery>>([]);
        }

        public Task<Delivery?> GetFirstOrDefaultAsync(Expression<Func<Delivery, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Delivery?> GetSingleOrDefaultAsync(Expression<Func<Delivery, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<Delivery>> GetAllAsync(Expression<Func<Delivery, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void PendingUpdate(Delivery entity) { }
        public void PendingUpdateRange(IReadOnlyCollection<Delivery> entities) { }
        public Task PendingAddAsync(Delivery entity, CancellationToken cancellationToken = default)
        {
            AddedDeliveries.Add(entity);
            return Task.CompletedTask;
        }
        public Task PendingAddRangeAsync(IReadOnlyCollection<Delivery> entities, CancellationToken cancellationToken = default)
        {
            AddedDeliveries.AddRange(entities);
            return Task.CompletedTask;
        }
        public void PendingRemove(Delivery entity) { }
        public void PendingRemoveRange(IReadOnlyList<Delivery> entities) { }
    }
}
