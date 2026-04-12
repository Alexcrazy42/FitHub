namespace FitHub.BankManager.Clients.Payment;

public interface IBankManagerPaymentClient
{
    Task<BankManagerPaymentIntentResult> CreatePaymentIntentAsync(
        string externalReference,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken ct);

    Task<BankManagerPaymentIntentResult> CompletePaymentIntentAsync(
        string paymentIntentId,
        bool succeeded,
        string externalEventId,
        string? failureReason,
        CancellationToken ct);

    Task<PublishOutboxMessagesResult> PublishOutboxAsync(CancellationToken ct);
}
