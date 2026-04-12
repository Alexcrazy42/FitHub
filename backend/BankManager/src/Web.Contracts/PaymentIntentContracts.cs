namespace FitHub.BankManager.Web.Contracts;

public record BankMoneyResponse(decimal Amount, string Currency);

public record CreatePaymentIntentRequest(
    string ExternalReference,
    decimal Amount,
    string Currency,
    string IdempotencyKey);

public record CompletePaymentIntentRequest(
    bool Succeeded,
    string ExternalEventId,
    string? FailureReason);

public record PaymentIntentResponse(
    string Id,
    string ExternalReference,
    BankMoneyResponse Amount,
    string Status,
    string? FailureReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record PublishOutboxMessagesResponse(int PublishedCount, int FailedCount);
