using FitHub.RabbitMQ.Contracts;

namespace FitHub.BankManager.Rabbit.Contracts.Payments;

public record PaymentStatusChangedMessage(
    string ReservationId,
    string PaymentIntentId,
    string Status,
    decimal Amount,
    string Currency,
    string? FailureReason) : IRabbitMqContract
{
    public static string ExchangeName => "platform";
    public static string ExchangeType => "direct";
    public static string DefaultRoutingKey => "payment.status.changed";
}
