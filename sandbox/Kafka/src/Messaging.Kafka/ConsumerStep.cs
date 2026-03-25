using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka;


public class ConsumerStep<TKey, TValue>
{
    private readonly IServiceCollection _services;
    private readonly KafkaBuilder _builder;

    public ConsumerStep(IServiceCollection services, KafkaBuilder builder)
    {
        _services = services;
        _builder = builder;
    }

    public KafkaBuilder WithConfig(Action<ConsumerConfig> configure)
    {
        _services.Configure<ConsumerOptions<TKey, TValue>>(o =>
        {
            configure(o.Config);
        });

        return _builder;
    }
}