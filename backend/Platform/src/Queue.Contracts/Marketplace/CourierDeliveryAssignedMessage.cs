using FitHub.RabbitMQ.Contracts;

namespace FitHub.Queue.Contracts.Marketplace;

public record CourierDeliveryAssignedMessage(
    string DeliveryId,
    string OrderId,
    string CourierId,
    DateTimeOffset AssignedAt,
    DateTimeOffset RespondBy) : IRabbitMqContract
{
    public static string ExchangeName => "marketplace.delivery";

    public static string ExchangeType => "direct";

    public static string DefaultRoutingKey => "delivery.courier.assigned";
}
