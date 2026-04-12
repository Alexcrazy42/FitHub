using FitHub.Common.Entities;

namespace FitHub.BankManager.Domain;

public class BankWebhookEvent : IEntity<BankWebhookEventId>
{
    private BankWebhookEvent(
        BankWebhookEventId id,
        string externalEventId,
        PaymentIntentId paymentIntentId,
        PaymentIntentStatus status,
        string? payload)
    {
        Id = id;
        ExternalEventId = externalEventId;
        PaymentIntentId = paymentIntentId;
        Status = status;
        Payload = payload;
        ReceivedAt = DateTimeOffset.UtcNow;
    }

    public BankWebhookEventId Id { get; }
    public string ExternalEventId { get; private set; }
    public PaymentIntentId PaymentIntentId { get; private set; }
    public PaymentIntent? PaymentIntent { get; private set; }
    public PaymentIntentStatus Status { get; private set; }
    public string? Payload { get; private set; }
    public DateTimeOffset ReceivedAt { get; }

    public static BankWebhookEvent Create(
        string externalEventId,
        PaymentIntentId paymentIntentId,
        PaymentIntentStatus status,
        string? payload = null)
    {
        if (String.IsNullOrWhiteSpace(externalEventId))
        {
            throw new ValidationException("ExternalEventId is required.");
        }

        return new BankWebhookEvent(BankWebhookEventId.New(), externalEventId, paymentIntentId, status, payload);
    }
}
