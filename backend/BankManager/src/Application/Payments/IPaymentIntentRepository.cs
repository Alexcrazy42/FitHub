using FitHub.BankManager.Domain;

namespace FitHub.BankManager.Application.Payments;

public interface IPaymentIntentRepository
{
    Task<PaymentIntent?> GetByIdAsync(PaymentIntentId id, CancellationToken ct);

    Task<PaymentIntent?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct);

    Task<IReadOnlyList<PaymentIntent>> GetAwaitingPaymentCreatedBeforeAsync(DateTimeOffset createdBefore, int batchSize, CancellationToken ct);

    Task<BankWebhookEvent?> GetWebhookEventAsync(string externalEventId, CancellationToken ct);

    Task AddPaymentIntentAsync(PaymentIntent intent, CancellationToken ct);

    Task AddWebhookEventAsync(BankWebhookEvent webhookEvent, CancellationToken ct);

    Task AddOutboxMessageAsync(RabbitOutboxMessage message, CancellationToken ct);
}
