using FitHub.Common.Entities;

namespace FitHub.Domain.Outbox;

public class RabbitOutboxMessage : IEntity<RabbitOutboxMessageId>
{
    private RabbitOutboxMessage(RabbitOutboxMessageId id, string exchangeName, string routingKey, string payload)
    {
        Id = id;
        ExchangeName = exchangeName;
        RoutingKey = routingKey;
        Payload = payload;
        Status = OutboxMessageStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public RabbitOutboxMessageId Id { get; }
    public string ExchangeName { get; private set; }
    public string RoutingKey { get; private set; }
    public string Payload { get; private set; }
    public OutboxMessageStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public string? Error { get; private set; }

    public void MarkPublished()
    {
        Status = OutboxMessageStatus.Published;
        PublishedAt = DateTimeOffset.UtcNow;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Status = OutboxMessageStatus.Failed;
        Error = error;
    }

    public static RabbitOutboxMessage Create(string exchangeName, string routingKey, string payload)
    {
        return new RabbitOutboxMessage(RabbitOutboxMessageId.New(), exchangeName, routingKey, payload);
    }
}
