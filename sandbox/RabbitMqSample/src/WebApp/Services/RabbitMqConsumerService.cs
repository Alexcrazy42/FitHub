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
    var consumer = new AsyncEventingBasicConsumer(channel);

    consumer.ReceivedAsync += async (sender, eventArgs) =>
    {
        try
        {
            byte[] body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($"📥 [{consumerTagPrefix}] Received: {message}");

            // 🧨 Искусственная ошибка: если сообщение содержит "fail" — бросаем исключение
            if (queueName == "direct.queue1" && message.Contains("fail"))
            {
                throw new InvalidOperationException($"Simulated failure for message: {message}");
            }

            // Если всё хорошо — подтверждаем
            await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            _logger.LogInformation($"✅ [{consumerTagPrefix}] Message acknowledged: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error processing message in [{consumerTagPrefix}]. Message will be NACKed or go to DLQ after retries.");

            // ❗ Важно: не делаем ACK → сообщение вернётся в очередь или уйдёт в DLQ, если достигнут лимит доставок
            // В нашем случае — так как нет настроек retry, RabbitMQ сразу отправит в DLQ (если очередь настроена с DLQ)
            // Но по умолчанию — без QoS и без настроек TTL/retry — сообщение может застрять.

            // 👇 Поэтому явно отклоняем сообщение без повторной постановки в очередь → сразу в DLQ
            try
            {
                await ((AsyncEventingBasicConsumer)sender).Channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false, // ← не возвращать в очередь → сразу в DLQ
                    cancellationToken: stoppingToken);
            }
            catch (Exception nackEx)
            {
                _logger.LogError(nackEx, "Failed to NACK message");
            }
        }
    };

    await channel.BasicConsumeAsync(
        queue: queueName,
        autoAck: false,
        consumer: consumer,
        cancellationToken: stoppingToken);

    _logger.LogInformation($"👂 Started consuming {queueName}");
}
}