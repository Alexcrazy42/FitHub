namespace FitHub.BankManager.Clients.Payment;

public record BankManagerPaymentIntentResult(
    string PaymentIntentId,
    string ExternalReference,
    decimal Amount,
    string Currency,
    string Status,
    string? FailureReason);

public record PublishOutboxMessagesResult(int PublishedCount, int FailedCount);

public record CompletePendingPaymentIntentsResult(int CompletedCount);
