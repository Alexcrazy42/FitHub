using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka;

public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;

    public KafkaProducer(IOptions<ProducerOptions<TKey, TValue>> options)
    {
        _producer = new ProducerBuilder<TKey, TValue>(options.Value.Config)
            .SetValueSerializer(new JsonSerializer<TValue>())
            .Build();
    }

    public async Task ProduceAsync(string topic, TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        await _producer.ProduceAsync(topic, new Message<TKey, TValue> { Key = key, Value = value }, cancellationToken);
    }
}