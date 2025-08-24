using Confluent.Kafka;

namespace Messaging.Kafka;

public class ConsumerOptions<TKey, TValue>
{
    public ConsumerConfig Config { get; set; } = new();
}
