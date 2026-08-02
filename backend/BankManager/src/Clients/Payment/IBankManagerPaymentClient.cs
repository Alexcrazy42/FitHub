using FitHub.BankManager.Web.Contracts;

namespace FitHub.BankManager.Clients.Payment;

public interface IBankManagerPaymentClient
{
    Task<BankManagerPaymentIntentResult> CreatePaymentIntentAsync(
        CreatePaymentIntentRequest request,
        CancellationToken ct);

    Task<BankManagerPaymentIntentResult> CompletePaymentIntentAsync(
        string paymentIntentId,
        bool succeeded,
        string externalEventId,
        string? failureReason,
        CancellationToken ct);

    Task<PublishOutboxMessagesResult> PublishOutboxAsync(CancellationToken ct);

    Task<CompletePendingPaymentIntentsResult> CompletePendingPaymentIntentsAsync(CancellationToken ct);
}
