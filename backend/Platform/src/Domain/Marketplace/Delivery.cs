using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class Delivery : IEntity<DeliveryId>
{
    private readonly List<DeliveryTrackingPoint> trackingPoints = [];
    private readonly List<DeliveryEvent> events = [];

    private Delivery(DeliveryId id, MarketplaceOrderId orderId, DateTimeOffset createdAt)
    {
        Id = id;
        OrderId = orderId;
        Status = DeliveryStatus.Pending;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        LastStateChangedAt = createdAt;
    }

    public DeliveryId Id { get; }
    public MarketplaceOrderId OrderId { get; private set; }
    public MarketplaceOrder? Order { get; private set; }
    public CourierId? CourierId { get; private set; }
    public Courier? Courier { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public string? PickupAddress { get; private set; }
    public string? DropoffAddress { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset LastStateChangedAt { get; private set; }
    public DateTimeOffset? LastCourierSignalAt { get; private set; }
    public DateTimeOffset? LastLocationAt { get; private set; }
    public DateTimeOffset? CourierAssignmentExpiresAt { get; private set; }
    public DateTimeOffset? WatchdogCheckedAt { get; private set; }
    public string? LastAutomaticDecisionReason { get; private set; }
    public IReadOnlyList<DeliveryTrackingPoint> TrackingPoints => trackingPoints;
    public IReadOnlyList<DeliveryEvent> Events => events;

    public void AssignCourier(Courier courier, DateTimeOffset assignmentExpiresAt)
    {
        if (Status is DeliveryStatus.Delivered or DeliveryStatus.Failed or DeliveryStatus.Cancelled)
        {
            throw new ValidationException($"Delivery {Id} cannot be assigned from final status {Status}.");
        }

        if (CourierId is not null && CourierId != courier.Id)
        {
            throw new ValidationException($"Delivery {Id} already has courier {CourierId}.");
        }

        CourierId = courier.Id;
        Courier = courier;
        CourierAssignmentExpiresAt = assignmentExpiresAt;
        courier.MarkBusy();
        ChangeStatus(DeliveryStatus.CourierAssigned, $"Курьер {courier.Name} назначен. Ожидаем подтверждение.");
    }

    public void AcceptByCourier(CourierId courierId)
    {
        EnsureAssignedCourier(courierId);
        CourierAssignmentExpiresAt = null;
        ChangeStatus(DeliveryStatus.Accepted, "Курьер принял доставку.");
    }

    public void RejectByCourier(CourierId courierId, string? reason = null)
    {
        EnsureAssignedCourier(courierId);
        Courier?.MarkAvailable();
        CourierId = null;
        Courier = null;
        CourierAssignmentExpiresAt = null;
        Status = DeliveryStatus.Pending;
        UpdatedAt = DateTimeOffset.UtcNow;
        LastStateChangedAt = UpdatedAt;
        LastAutomaticDecisionReason = reason;
        events.Add(DeliveryEvent.Create(Id, Status, reason ?? "Курьер отклонил доставку. Доставка вернулась в очередь назначения."));
    }

    public bool ExpireCourierAssignment(DateTimeOffset now, string? reason = null)
    {
        if (Status != DeliveryStatus.CourierAssigned ||
            CourierAssignmentExpiresAt is null ||
            CourierAssignmentExpiresAt > now)
        {
            return false;
        }

        Courier?.MarkAvailable();
        CourierId = null;
        Courier = null;
        CourierAssignmentExpiresAt = null;
        Status = DeliveryStatus.Pending;
        UpdatedAt = now;
        LastStateChangedAt = now;
        LastAutomaticDecisionReason = reason;
        events.Add(DeliveryEvent.Create(Id, Status, reason ?? "Курьер не подтвердил доставку вовремя. Доставка вернулась в очередь назначения."));
        return true;
    }

    public void ChangeStatus(DeliveryStatus status, string? message = null)
    {
        if (Status == status)
        {
            return;
        }

        if (Status is DeliveryStatus.Delivered or DeliveryStatus.Failed or DeliveryStatus.Cancelled)
        {
            throw new ValidationException($"Delivery {Id} cannot be changed from final status {Status}.");
        }

        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
        LastStateChangedAt = UpdatedAt;
        events.Add(DeliveryEvent.Create(Id, status, message));
    }

    public static Delivery CreateForOrder(MarketplaceOrderId orderId)
    {
        var delivery = new Delivery(DeliveryId.New(), orderId, DateTimeOffset.UtcNow);
        delivery.events.Add(DeliveryEvent.Create(delivery.Id, delivery.Status, "Доставка создана и ожидает сборки."));
        return delivery;
    }

    private void EnsureAssignedCourier(CourierId courierId)
    {
        if (Status != DeliveryStatus.CourierAssigned || CourierId != courierId)
        {
            throw new ValidationException($"Delivery {Id} is not assigned to courier {courierId}.");
        }
    }
}
