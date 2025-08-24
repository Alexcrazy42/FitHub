using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka;

public class KafkaConsumerService<TKey, TValue> : BackgroundService
{
    private readonly ILogger<KafkaConsumerService<TKey, TValue>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private readonly ConsumerOptions<TKey, TValue> _consumerOptions;
    private readonly KafkaConsumerOptions<TKey, TValue> _kafkaConsumerOptions;
    private IConsumer<TKey, TValue>? _consumer;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService<TKey, TValue>> logger,
        IServiceProvider serviceProvider,
        IConfiguration config,
        IOptions<ConsumerOptions<TKey, TValue>> consumerOptions,
        IOptions<KafkaConsumerOptions<TKey, TValue>> kafkaConsumerOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = config;
        _consumerOptions = consumerOptions.Value;
        _kafkaConsumerOptions = kafkaConsumerOptions.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var topic = _kafkaConsumerOptions.Topic;

        using var consumer = new ConsumerBuilder<TKey, TValue>(_consumerOptions.Config)
            .SetValueDeserializer(new JsonDeserializer<TValue>())
            .Build();

        _consumer = consumer;
        consumer.Subscribe(topic);

        _logger.LogInformation("Kafka consumer started for topic: {Topic}", topic);

        return Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
    }

    
    private async Task ConsumeLoop(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer started for topic: {Topic}", _kafkaConsumerOptions.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = _consumer.Consume(stoppingToken);
                if (cr is null || cr.IsPartitionEOF) continue;

                await HandleMessageAsync(_consumer, cr, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Kafka consumer loop");
                await Task.Delay(1000, stoppingToken);
            }
        }

        _consumer?.Close();
    }
    
    private async Task HandleMessageAsync(
        IConsumer<TKey, TValue> consumer,
        ConsumeResult<TKey, TValue> cr,
        CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TKey, TValue>>();

        try
        {
            await handler.HandleAsync(cr.Message.Key, cr.Message.Value, ct);
            consumer.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message {Topic} {Partition} {Offset}", cr.Topic, cr.Partition, cr.Offset);
            throw;
        }
    }
}