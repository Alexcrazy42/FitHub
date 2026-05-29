using System.Text;
using RabbitMQ.Client;

namespace WebApp.Services;

public class RabbitMqService : IDisposable
{
    private readonly Lazy<Task<IConnection>> _connectionFactory;
    private readonly Lazy<Task<IChannel>> _channelFactory;

    public RabbitMqService(IConfiguration configuration)
    {
        Console.WriteLine($"{configuration["RABBITMQ_HOST"]}, {configuration["RABBITMQ_PORT"]}, {configuration["RABBITMQ_USERNAME"]}, {configuration["RABBITMQ_PASSWORD"] }");
        
        var factory = new ConnectionFactory
        {
            HostName = configuration["RABBITMQ_HOST"] ?? "localhost",
            Port = int.Parse(configuration["RABBITMQ_PORT"] ?? "5672"),
            UserName = configuration["RABBITMQ_USERNAME"] ?? "guest",
            Password = configuration["RABBITMQ_PASSWORD"] ?? "guest"
        };

        _connectionFactory = new Lazy<Task<IConnection>>(() => factory.CreateConnectionAsync());
        _channelFactory = new Lazy<Task<IChannel>>(async () =>
        {
            var connection = await _connectionFactory.Value;
            var channel = await connection.CreateChannelAsync();
            await SetupExchangesAndQueues(channel);
            return channel;
        });
    }

    private async Task SetupExchangesAndQueues(IChannel channel)
    {
        // 1. Direct Exchange
        await channel.ExchangeDeclareAsync("demo.direct", ExchangeType.Direct);
        await SetupQueueWithDlq(channel, "direct.queue1", "demo.direct", "route1");
        await SetupQueueWithDlq(channel, "direct.queue2", "demo.direct", "route2");

        // 2. Fanout Exchange
        await channel.ExchangeDeclareAsync("demo.fanout", ExchangeType.Fanout);
        await SetupQueueWithDlq(channel, "fanout.queue1", "demo.fanout", "");
        await SetupQueueWithDlq(channel, "fanout.queue2", "demo.fanout", "");

        // 3. Topic Exchange
        await channel.ExchangeDeclareAsync("demo.topic", ExchangeType.Topic);
        await SetupQueueWithDlq(channel, "topic.queue.logs", "demo.topic", "*.log");
        await SetupQueueWithDlq(channel, "topic.queue.errors", "demo.topic", "#.error");

        Console.WriteLine("✅ Exchanges, queues, and DLQs declared.");
    }

    public async Task PublishDirect(string message, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var channel = await _channelFactory.Value;
        await channel.BasicPublishAsync("demo.direct", routingKey, body: body);
        Console.WriteLine($"📤 Published to direct exchange with key '{routingKey}': {message}");
    }

    public async Task PublishFanout(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var channel = await _channelFactory.Value;
        await channel.BasicPublishAsync("demo.fanout", "", body: body);
        Console.WriteLine($"📤 Published to fanout exchange: {message}");
    }

    public async Task PublishTopic(string message, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var channel = await _channelFactory.Value;
        await channel.BasicPublishAsync("demo.topic", routingKey, body: body);
        Console.WriteLine($"📤 Published to topic exchange with key '{routingKey}': {message}");
    }
    
    private async Task SetupQueueWithDlq(IChannel channel, string queueName, string exchange, string routingKey)
    {
        string dlqName = $"{queueName}.dlq";

        // 👇 1. ЯВНО создаём DLQ — иначе сообщения будут теряться!
        await channel.QueueDeclareAsync(
            queue: dlqName,
            durable: true,       // рекомендуется, чтобы сообщения не терялись при перезапуске
            exclusive: false,
            autoDelete: false,
            arguments: null      // можно добавить свои аргументы, например TTL для DLQ
        );

        // 2. Аргументы для основной очереди: указываем, куда слать "мёртвые" сообщения
        var args = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "" },           // используется default exchange, и тогда сообщение направляется напрямую в очередь, имя которой задаётся через x-dead-letter-routing-key
            { "x-dead-letter-routing-key", dlqName }    // Ключ — имя DLQ-очереди
        };

        // 3. Объявляем основную очередь с DLQ-настройками
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args
        );

        // 4. Биндинг основной очереди к exchange
        await channel.QueueBindAsync(queueName, exchange, routingKey);

        Console.WriteLine($"✅ Queue '{queueName}' declared with DLQ '{dlqName}'");
    }

    public async Task<IChannel> GetChannelAsync() => await _channelFactory.Value;

    public void Dispose()
    {
        if (_channelFactory.IsValueCreated)
        {
            var channel = _channelFactory.Value.GetAwaiter().GetResult();
            channel?.Dispose();
        }

        if (_connectionFactory.IsValueCreated)
        {
            var connection = _connectionFactory.Value.GetAwaiter().GetResult();
            connection?.Dispose();
        }
    }
}