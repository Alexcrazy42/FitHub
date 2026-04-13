using FitHub.Application.Outbox;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.Json;
using FitHub.Domain.Marketplace;
using FitHub.Domain.Outbox;
using FitHub.Queue.Contracts.Marketplace;

namespace FitHub.Application.Marketplace;

public class DeliveryAssignmentService : IDeliveryAssignmentService
{
    private readonly IDeliveryRepository deliveryRepository;
    private readonly ICourierRepository courierRepository;
    private readonly IOutboxRepository outboxRepository;
    private readonly IUnitOfWork unitOfWork;

    public DeliveryAssignmentService(
        IDeliveryRepository deliveryRepository,
        ICourierRepository courierRepository,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork)
    {
        this.deliveryRepository = deliveryRepository;
        this.courierRepository = courierRepository;
        this.outboxRepository = outboxRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<int> AutoAssignPendingAsync(int batchSize, TimeSpan acceptanceTimeout, CancellationToken ct)
    {
        var pendingDeliveries = await deliveryRepository.GetPendingForAssignmentAsync(batchSize, ct);

        if (pendingDeliveries.Count == 0)
        {
            return 0;
        }

        var availableCouriers = await courierRepository.GetAvailableForAssignmentAsync(pendingDeliveries.Count, ct);
        var assignedCount = 0;
        var now = DateTimeOffset.UtcNow;

        foreach (var pair in pendingDeliveries.Zip(availableCouriers))
        {
            var delivery = pair.First;
            var courier = pair.Second;
            var respondBy = now.Add(acceptanceTimeout);

            delivery.AssignCourier(courier, respondBy);
            await outboxRepository.AddAsync(
                RabbitOutboxMessage.Create(
                    CourierDeliveryAssignedMessage.ExchangeName,
                    CourierDeliveryAssignedMessage.DefaultRoutingKey,
                    CommonJsonSerializer.Serialize(new CourierDeliveryAssignedMessage(
                        delivery.Id.ToString(),
                        delivery.OrderId.ToString(),
                        courier.Id.ToString(),
                        now,
                        respondBy))),
                ct);
            assignedCount++;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return assignedCount;
    }

    public async Task<int> ReleaseExpiredAssignmentsAsync(DateTimeOffset now, int batchSize, CancellationToken ct)
    {
        var deliveries = await deliveryRepository.GetExpiredCourierAssignmentsAsync(now, batchSize, ct);
        var releasedCount = 0;

        foreach (var delivery in deliveries)
        {
            var courierId = delivery.CourierId;

            if (courierId is not null)
            {
                var courier = await courierRepository.GetByIdAsync(courierId, ct);
                courier?.MarkAvailable();
            }

            if (delivery.ExpireCourierAssignment(now))
            {
                releasedCount++;
            }
        }

        if (releasedCount > 0)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return releasedCount;
    }

    public async Task<int> EnsureCouriersAsync(IReadOnlyCollection<string> courierNames, CancellationToken ct)
    {
        var createdCount = 0;

        foreach (var courierName in courierNames.Where(x => !String.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var existingCourier = await courierRepository.GetByNameAsync(courierName, ct);

            if (existingCourier is not null)
            {
                continue;
            }

            await courierRepository.PendingAddAsync(Courier.Create(courierName), ct);
            createdCount++;
        }

        if (createdCount > 0)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return createdCount;
    }

    public async Task<Delivery> AcceptAssignmentAsync(DeliveryId deliveryId, CourierId courierId, CancellationToken ct)
    {
        var delivery = await GetDeliveryAsync(deliveryId, ct);
        delivery.AcceptByCourier(courierId);
        await unitOfWork.SaveChangesAsync(ct);
        return delivery;
    }

    public async Task<Delivery> RejectAssignmentAsync(DeliveryId deliveryId, CourierId courierId, string? reason, CancellationToken ct)
    {
        var delivery = await GetDeliveryAsync(deliveryId, ct);
        var courier = await courierRepository.GetByIdAsync(courierId, ct);
        delivery.RejectByCourier(courierId, reason);
        courier?.MarkAvailable();
        await unitOfWork.SaveChangesAsync(ct);
        return delivery;
    }

    private async Task<Delivery> GetDeliveryAsync(DeliveryId deliveryId, CancellationToken ct)
    {
        return await deliveryRepository.GetByIdAsync(deliveryId, ct)
               ?? throw new NotFoundException("Доставка не найдена.");
    }
}
