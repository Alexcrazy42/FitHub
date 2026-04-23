using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace.Deliveries;

public class DeliveryTrackingPoint : IEntity<DeliveryTrackingPointId>
{
    private DeliveryTrackingPoint(
        DeliveryTrackingPointId id,
        DeliveryId deliveryId,
        decimal latitude,
        decimal longitude,
        DateTimeOffset createdAt)
    {
        Id = id;
        DeliveryId = deliveryId;
        Latitude = latitude;
        Longitude = longitude;
        CreatedAt = createdAt;
    }

    public DeliveryTrackingPointId Id { get; }
    public DeliveryId DeliveryId { get; private set; }
    public Delivery? Delivery { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public static DeliveryTrackingPoint Create(DeliveryId deliveryId, decimal latitude, decimal longitude)
    {
        return new DeliveryTrackingPoint(DeliveryTrackingPointId.New(), deliveryId, latitude, longitude, DateTimeOffset.UtcNow);
    }
}
