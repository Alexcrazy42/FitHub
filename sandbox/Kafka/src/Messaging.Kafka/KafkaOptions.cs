using Confluent.Kafka;

namespace Messaging.Kafka;

public class KafkaOptions
{
    public string? BootstrapServers { get; set; }

    // Producer-specific
    public ProducerConfig Producer { get; set; } = new();

    // Consumer-specific
    public ConsumerConfig Consumer { get; set; } = new();
}