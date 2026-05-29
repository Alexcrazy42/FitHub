using Confluent.Kafka;

namespace Messaging.Kafka;

public class KafkaConsumerOptions<TKey, TValue>
{
    public string? Topic { get; set; }
    
    public string? GroupId { get; set; }
    public AutoOffsetReset OffsetReset { get; set; } = AutoOffsetReset.Latest;
    public int PollTimeoutMs { get; set; } = 100;
}