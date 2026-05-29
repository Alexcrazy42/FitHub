using FitHub.Common.Entities;

namespace FitHub.BankManager.Domain;

public class PaymentIntent : IEntity<PaymentIntentId>
{
    private readonly List<PaymentOperation> operations = [];

    private PaymentIntent(
        PaymentIntentId id,
        string externalReference,
        decimal amount,
        string currency,
        string idempotencyKey)
    {
        Id = id;
        ExternalReference = externalReference;
        Amount = amount;
        Currency = currency;
        IdempotencyKey = idempotencyKey;
        Status = PaymentIntentStatus.AwaitingPayment;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public PaymentIntentId Id { get; }
    public BankAccountId? BankAccountId { get; private set; }
    public BankAccount? BankAccount { get; private set; }
    public string ExternalReference { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public PaymentIntentStatus Status { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public long Version { get; private set; }
    public IReadOnlyList<PaymentOperation> Operations => operations;

    public void MarkPaid(string externalEventId)
    {
        if (Status == PaymentIntentStatus.Paid)
        {
            return;
        }

        if (Status is PaymentIntentStatus.Failed or PaymentIntentStatus.Expired or PaymentIntentStatus.Cancelled)
        {
            throw new ValidationException($"PaymentIntent {Id} cannot be paid from status {Status}.");
        }

        Status = PaymentIntentStatus.Paid;
        PaidAt = DateTimeOffset.UtcNow;
        FailureReason = null;
        operations.Add(PaymentOperation.Create(Id, PaymentOperationType.Capture, PaymentOperationStatus.Succeeded, externalEventId));
        Touch();
    }

    public void MarkFailed(string externalEventId, string? failureReason)
    {
        if (Status == PaymentIntentStatus.Failed)
        {
            return;
        }

        if (Status is PaymentIntentStatus.Paid or PaymentIntentStatus.Expired or PaymentIntentStatus.Cancelled)
        {
            throw new ValidationException($"PaymentIntent {Id} cannot fail from status {Status}.");
        }

        Status = PaymentIntentStatus.Failed;
        FailedAt = DateTimeOffset.UtcNow;
        FailureReason = failureReason;
        operations.Add(PaymentOperation.Create(Id, PaymentOperationType.Capture, PaymentOperationStatus.Failed, externalEventId, failureReason));
        Touch();
    }

    public void MarkExpired()
    {
        if (Status is PaymentIntentStatus.Paid or PaymentIntentStatus.Failed)
        {
            return;
        }

        Status = PaymentIntentStatus.Expired;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        Version++;
    }

    public static PaymentIntent Create(string externalReference, decimal amount, string currency, string idempotencyKey)
    {
        if (String.IsNullOrWhiteSpace(externalReference))
        {
            throw new ValidationException("ExternalReference is required.");
        }

        if (amount <= 0)
        {
            throw new ValidationException("Payment amount must be positive.");
        }

        if (String.IsNullOrWhiteSpace(currency))
        {
            throw new ValidationException("Currency is required.");
        }

        if (String.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ValidationException("IdempotencyKey is required.");
        }

        return new PaymentIntent(PaymentIntentId.New(), externalReference, amount, currency, idempotencyKey);
    }
}
