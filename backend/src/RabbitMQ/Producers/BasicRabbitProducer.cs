using System.Text;
using System.Text.Json;
using FitHub.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace FitHub.RabbitMQ.Producers;

public class BasicRabbitProducer<TOptions> : IBasicProducer<TOptions>
    where TOptions : class, IRabbitMqOptions
{
    private readonly Lazy<Task<IChannel>> channelFactory;

    public BasicRabbitProducer(IRabbitMqConnection<TOptions> connection)
    {
        channelFactory = new Lazy<Task<IChannel>>(async () =>
        {
            var channel = await connection.CreateChannelAsync();
            return channel;
        });
    }

    public async Task BasicPublishAsync(string exchangeName, object message, string routingKey, CancellationToken ct = default)
    {
        var channel = await channelFactory.Value;
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var publishTimeout = TimeSpan.FromSeconds(5);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(publishTimeout);

        try
        {
            await channel.BasicPublishAsync(exchangeName, routingKey, body: body, cancellationToken: cts.Token);
        }
        catch (OperationCanceledException)
        {
            if (cts.Token.IsCancellationRequested)
            {
                throw new TimeoutException($"Message publishing timed out after {publishTimeout.TotalSeconds} seconds.");
            }
            throw;
        }
    }
}

