using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApp.Services;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly RabbitMqService _rabbitMqService;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    public RabbitMqConsumerService(RabbitMqService rabbitMqService, ILogger<RabbitMqConsumerService> logger)
    {
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _rabbitMqService.GetChannelAsync();

        // Запускаем потребителей параллельно
        _ = ConsumeQueueAsync(channel, "direct.queue1", "Direct Queue 1", stoppingToken);
        _ = ConsumeQueueAsync(channel, "direct.queue2", "Direct Queue 2", stoppingToken);
        _ = ConsumeQueueAsync(channel, "fanout.queue1", "Fanout Queue 1", stoppingToken);
        _ = ConsumeQueueAsync(channel, "fanout.queue2", "Fanout Queue 2", stoppingToken);
        _ = ConsumeQueueAsync(channel, "topic.queue.logs", "Topic Logs Queue", stoppingToken);
        _ = ConsumeQueueAsync(channel, "topic.queue.errors", "Topic Errors Queue", stoppingToken);

        // Держим сервис запущенным
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ConsumeQueueAsync(IChannel channel, string queueName, string consumerTagPrefix, CancellationToken stoppingToken)
    {
        // Создаём consumer ДО вызова BasicConsumeAsync
        var consumer = new AsyncEventingBasicConsumer(channel);

        // Подписываемся на событие получения сообщения
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                byte[] body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"📥 [{consumerTagPrefix}] Received: {message}");
                Console.WriteLine($"📥 [{consumerTagPrefix}] Received: {message}");
                await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error processing message in [{consumerTagPrefix}]");
            }
            await Task.CompletedTask;
        };

        // Запускаем потребление
        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation($"👂 Started consuming {queueName}");
    }
}