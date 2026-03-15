using System.Text;
using FitHub.Common.Entities;
using FitHub.Common.Json;
using FitHub.RabbitMQ.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FitHub.RabbitMQ.Producers;

public class BasicRabbitProducer<TOptions> : IBasicProducer<TOptions>
    where TOptions : class, IRabbitMqOptions
{
    private readonly Lazy<Task<IChannel>> channelFactory;
    private readonly ILogger<BasicRabbitProducer<TOptions>> logger;

    public BasicRabbitProducer(IRabbitMqConnection<TOptions> connection, ILogger<BasicRabbitProducer<TOptions>> logger)
    {
        this.logger = logger;
        channelFactory = new Lazy<Task<IChannel>>(async () =>
        {
            var channel = await connection.CreateChannelAsync();

            channel.BasicReturnAsync += (_, args) =>
            {
                logger.LogWarning("Unrouted message, replyKey={ReplyKey}, replyText={ReplyText}, routingKey={RoutingKey}", args.ReplyCode, args.ReplyText, args.RoutingKey);

                throw new UnexpectedException($"Unrouted message, replyKey={args.ReplyCode}, replyText={args.ReplyText}, routingKey={args.RoutingKey}");
            };

            return channel;
        });
    }

    public async Task BasicPublishAsync(string exchangeName, object message, string routingKey, CancellationToken ct = default)
    {
        var channel = await channelFactory.Value;
        var body = Encoding.UTF8.GetBytes(CommonJsonSerializer.Serialize(message));

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

