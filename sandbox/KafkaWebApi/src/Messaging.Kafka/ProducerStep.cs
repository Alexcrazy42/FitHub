using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka;


public class ProducerStep<TKey, TValue>
{
    private readonly IServiceCollection _services;
    private readonly KafkaBuilder _builder;

    public ProducerStep(IServiceCollection services, KafkaBuilder builder)
    {
        _services = services;
        _builder = builder;
    }

    public KafkaBuilder WithConfig(Action<ProducerConfig> configure)
    {
        _services.Configure<ProducerOptions<TKey, TValue>>(o =>
        {
            configure(o.Config);
        });

        _services.AddSingleton<IKafkaProducer<TKey, TValue>, KafkaProducer<TKey, TValue>>();
        
        return _builder;
    }
}