using FitHub.BankManager.Clients;
using FitHub.BankManager.Clients.Payment;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.RabbitMQ.Consumers;

namespace FitHub.BankManager.Jobs.Consumers.Payments;

[Consumer("bank-manager.payment-intent.requested.queue", "payment-intent.requested")]
public sealed class PaymentIntentRequestedConsumer : IRabbitMqConsumerHandler<PaymentIntentRequestedMessage>
{
    private readonly IBankManagerPaymentClient paymentClient;

    public PaymentIntentRequestedConsumer(IBankManagerPaymentClient paymentClient)
    {
        this.paymentClient = paymentClient;
    }

    public Task HandleAsync(PaymentIntentRequestedMessage message, CancellationToken ct)
    {
        return paymentClient.CreatePaymentIntentAsync(
            message.ReservationId,
            message.Amount,
            message.Currency,
            message.IdempotencyKey,
            ct);
    }
}
