using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace.Deliveries;

public class DeliveryEvent : IEntity<DeliveryEventId>
{
    private DeliveryEvent(DeliveryEventId id, DeliveryId deliveryId, DeliveryStatus status, DateTimeOffset createdAt)
    {
        Id = id;
        DeliveryId = deliveryId;
        Status = status;
        CreatedAt = createdAt;
    }

    public DeliveryEventId Id { get; }
    public DeliveryId DeliveryId { get; private set; }
    public Delivery? Delivery { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public string? Message { get; private set; }

    public static DeliveryEvent Create(DeliveryId deliveryId, DeliveryStatus status, string? message = null)
    {
        return new DeliveryEvent(DeliveryEventId.New(), deliveryId, status, DateTimeOffset.UtcNow)
        {
            Message = message
        };
    }
}
