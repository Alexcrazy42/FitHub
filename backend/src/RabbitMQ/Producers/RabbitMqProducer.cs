using System.Text;
using FitHub.Common.Entities;
using FitHub.Common.Json;
using FitHub.Common.Utilities.System;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FitHub.RabbitMQ.Producers;

/// <summary>
/// Базовый класс для продюсера рэббита
/// </summary>
public class RabbitMqProducer<TMessage, TProducer, TOptions> : IRabbitProducer<TMessage>
    where TMessage : class, IRabbitMqContract
    where TOptions : class, IRabbitMqOptions
{
    private readonly Lazy<Task<IChannel>> channelFactory;
    private readonly ILogger<RabbitMqProducer<TMessage, TProducer, TOptions>> logger;
    private readonly string exchangeName;
    private readonly string exchangeType;
    private readonly string defaultRoutingKey;


    public RabbitMqProducer(IRabbitMqConnection<TOptions> connection,
        IOptions<TOptions> rabbitOptions,
        ILogger<RabbitMqProducer<TMessage, TProducer, TOptions>> logger)
    {
        this.logger = logger;
        var producerAttribute = (ProducerAttribute)Attribute.GetCustomAttribute(typeof(TProducer), typeof(ProducerAttribute)).Required();
        if (producerAttribute is null)
        {
            throw new ArgumentException("Producer attribute is not defined on the producer attribute");
        }

        exchangeName = TMessage.ExchangeName;
        exchangeType = producerAttribute.ExchangeType.Required();
        defaultRoutingKey = TMessage.DefaultRoutingKey;
        channelFactory = new Lazy<Task<IChannel>>(async () =>
        {
            var channel = await connection.CreateChannelAsync();
            if (rabbitOptions.Value.NeedToPrepare)
            {
                await SetupExchangeAsync(channel);
            }

            channel.BasicReturnAsync += (_, args) =>
            {
                logger.LogWarning("Unrouted message, replyKey={ReplyKey}, replyText={ReplyText}, routingKey={RoutingKey}", args.ReplyCode, args.ReplyText, args.RoutingKey);

                throw new UnexpectedException($"Unrouted message, replyKey={args.ReplyCode}, replyText={args.ReplyText}, routingKey={args.RoutingKey}");
            };

            return channel;
        });
    }

    private async Task SetupExchangeAsync(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true, autoDelete: false);
    }

    public async Task BasicPublishAsync(TMessage message, CancellationToken ct)
    {
        var channel = await channelFactory.Value;
        var body = Encoding.UTF8.GetBytes(CommonJsonSerializer.Serialize(message));
        var publishTimeout = TimeSpan.FromSeconds(5);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(publishTimeout);

        try
        {
            await channel.BasicPublishAsync(
                exchangeName,
                defaultRoutingKey,
                body: body,
                mandatory: true,
                cancellationToken: cts.Token);
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

    public async Task BasicPublishAsync(string routingKey, TMessage message, CancellationToken ct = default)
    {
        var channel = await channelFactory.Value;
        var body = Encoding.UTF8.GetBytes(CommonJsonSerializer.Serialize(message));
        var publishTimeout = TimeSpan.FromSeconds(5);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(publishTimeout);

        try
        {
            await channel.BasicPublishAsync(
                exchangeName,
                routingKey,
                body: body,
                mandatory: true,
                cancellationToken: cts.Token);
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

