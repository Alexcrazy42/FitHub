namespace FitHub.Domain.Marketplace.Deliveries;

public enum DeliveryStatus
{
    Pending,
    Assembling,
    CourierAssigned,
    Accepted,
    PickedUp,
    InTransit,
    Delivered,
    Failed,
    Cancelled
}
