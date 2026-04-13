namespace FitHub.Domain.Marketplace;

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
