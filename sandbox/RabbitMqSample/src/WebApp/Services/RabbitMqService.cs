using System.Text;
using RabbitMQ.Client;

namespace WebApp.Services;

public class RabbitMqService : IDisposable
{
    private readonly Lazy<Task<IConnection>> _connectionFactory;
    private readonly Lazy<Task<IChannel>> _channelFactory;

    public RabbitMqService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RABBITMQ_HOST"] ?? throw new Exception("RABBITMQ_HOST"),
            Port = int.Parse(configuration["RABBITMQ_PORT"] ?? throw new Exception("RABBITMQ_PORT")),
            UserName = configuration["RABBITMQ_USERNAME"] ?? throw new Exception("RABBITMQ_USERNAME"),
            Password = configuration["RABBITMQ_PASSWORD"] ?? throw new Exception("RABBITMQ_PASSWORD")
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
        await channel.QueueDeclareAsync("direct.queue1", durable: false, exclusive: false, autoDelete: false);
        await channel.QueueDeclareAsync("direct.queue2", durable: false, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("direct.queue1", "demo.direct", "route1");
        await channel.QueueBindAsync("direct.queue2", "demo.direct", "route2");

        // 2. Fanout Exchange
        await channel.ExchangeDeclareAsync("demo.fanout", ExchangeType.Fanout);
        await channel.QueueDeclareAsync("fanout.queue1", durable: false, exclusive: false, autoDelete: false);
        await channel.QueueDeclareAsync("fanout.queue2", durable: false, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("fanout.queue1", "demo.fanout", "");
        await channel.QueueBindAsync("fanout.queue2", "demo.fanout", "");

        // 3. Topic Exchange
        await channel.ExchangeDeclareAsync("demo.topic", ExchangeType.Topic);
        await channel.QueueDeclareAsync("topic.queue.logs", durable: false, exclusive: false, autoDelete: false);
        await channel.QueueDeclareAsync("topic.queue.errors", durable: false, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("topic.queue.logs", "demo.topic", "*.log");
        await channel.QueueBindAsync("topic.queue.errors", "demo.topic", "#.error");

        Console.WriteLine("✅ Exchanges and queues declared.");
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