using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka;


public class KafkaBuilder
{
    private readonly IServiceCollection _services;

    public KafkaBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public ProducerStep<TKey, TValue> AddProducer<TKey, TValue>()
    {
        return new ProducerStep<TKey, TValue>(_services, this);
    }

    public ConsumerStep<TKey, TValue> AddConsumer<THandler, TKey, TValue>(string topic)
        where THandler : class, IMessageHandler<TKey, TValue>
    {
        _services.AddSingleton<IMessageHandler<TKey, TValue>, THandler>();

        _services.Configure<KafkaConsumerOptions<TKey, TValue>>(o =>
        {
            o.Topic = topic;
        });

        _services.AddHostedService<KafkaConsumerService<TKey, TValue>>();

        return new ConsumerStep<TKey, TValue>(_services, this);
    }
    
    public void Build()
    {
        // Можно сохранить options в DI, если нужно
    }
}