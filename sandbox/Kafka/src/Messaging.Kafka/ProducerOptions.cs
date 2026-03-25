using Confluent.Kafka;

namespace Messaging.Kafka;

public class ProducerOptions<TKey, TValue>
{
    public ProducerConfig Config { get; set; } = new();
}
