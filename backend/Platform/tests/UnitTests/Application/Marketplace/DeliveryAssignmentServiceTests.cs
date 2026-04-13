using System.Linq.Expressions;
using FitHub.Application.Common;
using FitHub.Application.Marketplace;
using FitHub.Application.Outbox;
using FitHub.Authentication;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Outbox;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Application.Marketplace;

public class DeliveryAssignmentServiceTests
{
    [Fact(DisplayName = "Auto assignment assigns one courier to one delivery")]
    public async Task AutoAssignPendingAsync_ShouldAssignCourierAndPublishEvent()
    {
        var delivery = Delivery.CreateForOrder(MarketplaceOrderId.New());
        var courier = Courier.Create("Test courier");
        var deliveryRepository = new FakeDeliveryRepository { PendingDeliveries = [delivery] };
        var courierRepository = new FakeCourierRepository { AvailableCouriers = [courier] };
        var outboxRepository = new FakeOutboxRepository();
        var service = CreateService(deliveryRepository, courierRepository, outboxRepository);

        var assignedCount = await service.AutoAssignPendingAsync(10, TimeSpan.FromSeconds(20), CancellationToken.None);

        assignedCount.ShouldBe(1);
        delivery.Status.ShouldBe(DeliveryStatus.CourierAssigned);
        delivery.CourierId.ShouldBe(courier.Id);
        delivery.CourierAssignmentExpiresAt.ShouldNotBeNull();
        courier.IsAvailable.ShouldBeFalse();
        outboxRepository.Messages.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Auto assignment does not assign one courier to two deliveries")]
    public async Task AutoAssignPendingAsync_ShouldNotAssignOneCourierToTwoDeliveries()
    {
        var firstDelivery = Delivery.CreateForOrder(MarketplaceOrderId.New());
        var secondDelivery = Delivery.CreateForOrder(MarketplaceOrderId.New());
        var courier = Courier.Create("Test courier");
        var deliveryRepository = new FakeDeliveryRepository { PendingDeliveries = [firstDelivery, secondDelivery] };
        var courierRepository = new FakeCourierRepository { AvailableCouriers = [courier] };
        var service = CreateService(deliveryRepository, courierRepository, new FakeOutboxRepository());

        var assignedCount = await service.AutoAssignPendingAsync(10, TimeSpan.FromSeconds(20), CancellationToken.None);

        assignedCount.ShouldBe(1);
        firstDelivery.CourierId.ShouldBe(courier.Id);
        secondDelivery.CourierId.ShouldBeNull();
        courier.IsAvailable.ShouldBeFalse();
    }

    [Fact(DisplayName = "Expired courier assignment returns delivery to assignment queue")]
    public async Task ReleaseExpiredAssignmentsAsync_ShouldReturnDeliveryToQueue()
    {
        var delivery = Delivery.CreateForOrder(MarketplaceOrderId.New());
        var courier = Courier.Create("Test courier");
        var now = DateTimeOffset.UtcNow;
        delivery.AssignCourier(courier, now.AddSeconds(-1));
        var deliveryRepository = new FakeDeliveryRepository { ExpiredAssignments = [delivery] };
        var courierRepository = new FakeCourierRepository { CourierById = courier };
        var service = CreateService(deliveryRepository, courierRepository, new FakeOutboxRepository());

        var releasedCount = await service.ReleaseExpiredAssignmentsAsync(now, 10, CancellationToken.None);

        releasedCount.ShouldBe(1);
        delivery.Status.ShouldBe(DeliveryStatus.Pending);
        delivery.CourierId.ShouldBeNull();
        courier.IsAvailable.ShouldBeTrue();
    }

    [Fact(DisplayName = "Courier accept moves delivery to accepted")]
    public async Task AcceptAssignmentAsync_ShouldMoveDeliveryToAccepted()
    {
        var delivery = Delivery.CreateForOrder(MarketplaceOrderId.New());
        var courier = Courier.Create("Test courier");
        delivery.AssignCourier(courier, DateTimeOffset.UtcNow.AddSeconds(20));
        var service = CreateService(
            new FakeDeliveryRepository { DeliveryById = delivery },
            new FakeCourierRepository { CourierById = courier },
            new FakeOutboxRepository());

        var result = await service.AcceptAssignmentAsync(delivery.Id, courier.Id, CancellationToken.None);

        result.Status.ShouldBe(DeliveryStatus.Accepted);
        result.CourierId.ShouldBe(courier.Id);
        result.CourierAssignmentExpiresAt.ShouldBeNull();
        courier.IsAvailable.ShouldBeFalse();
    }

    [Fact(DisplayName = "Courier reject returns delivery to pending")]
    public async Task RejectAssignmentAsync_ShouldReturnDeliveryToPending()
    {
        var delivery = Delivery.CreateForOrder(MarketplaceOrderId.New());
        var courier = Courier.Create("Test courier");
        delivery.AssignCourier(courier, DateTimeOffset.UtcNow.AddSeconds(20));
        var service = CreateService(
            new FakeDeliveryRepository { DeliveryById = delivery },
            new FakeCourierRepository { CourierById = courier },
            new FakeOutboxRepository());

        var result = await service.RejectAssignmentAsync(delivery.Id, courier.Id, "debug reject", CancellationToken.None);

        result.Status.ShouldBe(DeliveryStatus.Pending);
        result.CourierId.ShouldBeNull();
        result.CourierAssignmentExpiresAt.ShouldBeNull();
        courier.IsAvailable.ShouldBeTrue();
    }

    private static DeliveryAssignmentService CreateService(
        FakeDeliveryRepository deliveryRepository,
        FakeCourierRepository courierRepository,
        FakeOutboxRepository outboxRepository)
    {
        return new DeliveryAssignmentService(
            deliveryRepository,
            courierRepository,
            outboxRepository,
            new FakeUnitOfWork());
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
        public List<RabbitOutboxMessage> Messages { get; } = [];

        public Task AddAsync(RabbitOutboxMessage message, CancellationToken ct)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<RabbitOutboxMessage>> GetPendingAsync(int batchSize, CancellationToken ct)
        {
            return Task.FromResult<IReadOnlyList<RabbitOutboxMessage>>([]);
        }
    }

    private sealed class FakeCourierRepository : ICourierRepository
    {
        public IReadOnlyList<Courier> AvailableCouriers { get; init; } = [];
        public Courier? CourierById { get; init; }

        public Task<Courier?> GetByIdAsync(CourierId courierId, CancellationToken ct)
        {
            return Task.FromResult(CourierById?.Id == courierId ? CourierById : AvailableCouriers.FirstOrDefault(x => x.Id == courierId));
        }

        public Task<Courier?> GetByNameAsync(string name, CancellationToken ct)
        {
            return Task.FromResult(AvailableCouriers.FirstOrDefault(x => x.Name == name));
        }

        public Task<IReadOnlyList<Courier>> GetAvailableForAssignmentAsync(int take, CancellationToken ct)
        {
            return Task.FromResult(AvailableCouriers.Take(take).ToList() as IReadOnlyList<Courier>);
        }

        public Task<Courier?> GetFirstOrDefaultAsync(Expression<Func<Courier, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Courier?> GetSingleOrDefaultAsync(Expression<Func<Courier, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<Courier>> GetAllAsync(Expression<Func<Courier, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void PendingUpdate(Courier entity) { }
        public void PendingUpdateRange(IReadOnlyCollection<Courier> entities) { }
        public Task PendingAddAsync(Courier entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PendingAddRangeAsync(IReadOnlyCollection<Courier> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void PendingRemove(Courier entity) { }
        public void PendingRemoveRange(IReadOnlyList<Courier> entities) { }
    }

    private sealed class FakeDeliveryRepository : IDeliveryRepository
    {
        public IReadOnlyList<Delivery> PendingDeliveries { get; init; } = [];
        public IReadOnlyList<Delivery> ExpiredAssignments { get; init; } = [];
        public Delivery? DeliveryById { get; init; }

        public Task<Delivery?> GetByIdAsync(DeliveryId deliveryId, CancellationToken ct)
        {
            return Task.FromResult(DeliveryById?.Id == deliveryId ? DeliveryById : PendingDeliveries.FirstOrDefault(x => x.Id == deliveryId));
        }

        public Task<Delivery?> GetByOrderIdAsync(MarketplaceOrderId orderId, CancellationToken ct) => Task.FromResult<Delivery?>(null);
        public Task<Delivery?> GetByOrderIdForUserAsync(MarketplaceOrderId orderId, IdentityUserId userId, CancellationToken ct) => Task.FromResult<Delivery?>(null);
        public Task<PagedResult<Delivery>> GetAllAsync(PagedQuery paged, CancellationToken ct) => Task.FromResult(PagedResult<Delivery>.Create([]));
        public Task<IReadOnlyList<Delivery>> GetPendingForAssignmentAsync(int take, CancellationToken ct) => Task.FromResult(PendingDeliveries.Take(take).ToList() as IReadOnlyList<Delivery>);
        public Task<IReadOnlyList<Delivery>> GetExpiredCourierAssignmentsAsync(DateTimeOffset now, int take, CancellationToken ct) => Task.FromResult(ExpiredAssignments.Take(take).ToList() as IReadOnlyList<Delivery>);
        public Task<Delivery?> GetFirstOrDefaultAsync(Expression<Func<Delivery, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Delivery?> GetSingleOrDefaultAsync(Expression<Func<Delivery, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<Delivery>> GetAllAsync(Expression<Func<Delivery, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void PendingUpdate(Delivery entity) { }
        public void PendingUpdateRange(IReadOnlyCollection<Delivery> entities) { }
        public Task PendingAddAsync(Delivery entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PendingAddRangeAsync(IReadOnlyCollection<Delivery> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void PendingRemove(Delivery entity) { }
        public void PendingRemoveRange(IReadOnlyList<Delivery> entities) { }
    }
}
