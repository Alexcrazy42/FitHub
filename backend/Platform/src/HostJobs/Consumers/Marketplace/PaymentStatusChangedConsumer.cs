using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.Clients.Marketplace;
using FitHub.Contracts.V1.Marketplace;
using FitHub.RabbitMQ.Consumers;

namespace FitHub.HostJobs.Consumers.Marketplace;

[Consumer("platform.payment.status.changed.queue", "payment.status.changed")]
public sealed class PaymentStatusChangedConsumer : IRabbitMqConsumerHandler<PaymentStatusChangedMessage>
{
    private readonly IMarketplaceJobsClient marketplaceJobsClient;

    public PaymentStatusChangedConsumer(IMarketplaceJobsClient marketplaceJobsClient)
    {
        this.marketplaceJobsClient = marketplaceJobsClient;
    }

    public Task HandleAsync(PaymentStatusChangedMessage message, CancellationToken ct)
    {
        return marketplaceJobsClient.ApplyBankPaymentStatusAsync(
            new ApplyBankPaymentStatusRequest(
                message.ReservationId,
                message.PaymentIntentId,
                message.Status,
                message.Amount,
                message.Currency,
                message.FailureReason),
            ct);
    }
}
