namespace Messaging.Kafka;

public interface IMessageHandler<in TKey, in TValue>
{
    Task HandleAsync(TKey key, TValue value, CancellationToken ct);
}