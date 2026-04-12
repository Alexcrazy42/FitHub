using FitHub.Common.Entities;

namespace FitHub.BankManager.Domain;

public class PaymentOperation : IEntity<PaymentOperationId>
{
    private PaymentOperation(
        PaymentOperationId id,
        PaymentIntentId paymentIntentId,
        PaymentOperationType type,
        PaymentOperationStatus status,
        string externalEventId,
        string? failureReason)
    {
        Id = id;
        PaymentIntentId = paymentIntentId;
        Type = type;
        Status = status;
        ExternalEventId = externalEventId;
        FailureReason = failureReason;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public PaymentOperationId Id { get; }
    public PaymentIntentId PaymentIntentId { get; private set; }
    public PaymentIntent? PaymentIntent { get; private set; }
    public PaymentOperationType Type { get; private set; }
    public PaymentOperationStatus Status { get; private set; }
    public string ExternalEventId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public static PaymentOperation Create(
        PaymentIntentId paymentIntentId,
        PaymentOperationType type,
        PaymentOperationStatus status,
        string externalEventId,
        string? failureReason = null)
    {
        return new PaymentOperation(PaymentOperationId.New(), paymentIntentId, type, status, externalEventId, failureReason);
    }
}
