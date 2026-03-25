using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static KafkaBuilder AddKafka(this IServiceCollection services)
    {
        return new KafkaBuilder(services);
    }
}