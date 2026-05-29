using FitHub.RabbitMQ.Contracts;

namespace FitHub.BankManager.Rabbit.Contracts.Payments;

public record PaymentIntentRequestedMessage(
    string ReservationId,
    decimal Amount,
    string Currency,
    string IdempotencyKey) : IRabbitMqContract
{
    public static string ExchangeName => "bank-manager";
    public static string ExchangeType => "direct";
    public static string DefaultRoutingKey => "payment-intent.requested";
}
