using System.Text;
using FitHub.Common.Json;
using FitHub.Common.Utilities.System;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FitHub.RabbitMQ.Consumers;

internal sealed class RabbitMqConsumer<TMessage, TOptions> : BackgroundService
    where TMessage : class, IRabbitMqContract
    where TOptions : class, IRabbitMqOptions
{
    private readonly ILogger<RabbitMqConsumer<TMessage, TOptions>> logger;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IOptions<TOptions> rabbitOptions;

    private string? exchangeName;
    private string? queueName;
    private string? routingKey;
    private Lazy<Task<IChannel>>? channelFactory;

    public RabbitMqConsumer(IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumer<TMessage, TOptions>> logger, IOptions<TOptions> rabbitOptions)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
        this.rabbitOptions = rabbitOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var scope = scopeFactory.CreateScope();
            var connection = scope.ServiceProvider.GetRequiredService<IRabbitMqConnection<TOptions>>();
            var handler = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumerHandler<TMessage>>();

            var handlerType = handler.GetType();

            var consumerAttribute = (ConsumerAttribute)Attribute.GetCustomAttribute(handlerType, typeof(ConsumerAttribute)).Required();

            if (consumerAttribute == null)
            {
                throw new ArgumentException("Consumer attribute is not defined on the consumer");
            }

            exchangeName = TMessage.ExchangeName;
            queueName = consumerAttribute.QueueName;
            routingKey = consumerAttribute.BindingRoutingKey;

            channelFactory = new Lazy<Task<IChannel>>(async () =>
            {
                var channel = await connection.CreateChannelAsync();
                if (rabbitOptions.Value.NeedToPrepare)
                {
                    await SetupQueueWithBindingsAsync(channel);
                }
                return channel;
            });

            await ConsumeQueueAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    private async Task SetupQueueWithBindingsAsync(IChannel channel)
    {
        await SetupQueueWithDlq(channel);
    }

    private async Task SetupQueueWithDlq(IChannel channel)
    {
        var dlqName = $"{queueName}_DeadLetter";

        var argsForDlq = new Dictionary<string, object?>
        {
            { "x-queue-type", "quorum" }
        };

        await channel.QueueDeclareAsync(dlqName, durable: true, exclusive: false, autoDelete: false, arguments: argsForDlq);

        var args = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", dlqName },
            { "x-queue-type", "quorum" }
        };

        await channel.QueueDeclareAsync(queueName.Required(), durable: true, exclusive: false, autoDelete: false, arguments: args);
        await channel.QueueBindAsync(queueName, exchangeName.Required(), routingKey.Required());

        logger.LogInformation("{QueueName} declared with DLQ {DLQName}", queueName, dlqName);
    }

    private async Task ConsumeQueueAsync(CancellationToken ct)
    {
        var maxRetryAttempts = 5;
        var delayBetweenAttempts = 2000;

        for (var attempt = 1; attempt <= maxRetryAttempts; attempt++)
        {
            try
            {
                var channel = await channelFactory.Required().Value;
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var messageString = Encoding.UTF8.GetString(body);
                        var message = CommonJsonSerializer.Deserialize<TMessage>(messageString).Required();
                        logger.LogInformation("{QueueName} Message received: {Message}", queueName, message);

                        var scope = scopeFactory.CreateScope();
                        using (scope)
                        {
                            var handler = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumerHandler<TMessage>>();
                            await handler.HandleAsync(message, ct);
                        }

                        await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: ct);
                        logger.LogInformation("{QueueName} Message ack: {Message}", queueName, message);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed while consume message from {QueueName}", queueName);
                        try
                        {
                            await channel.BasicNackAsync(
                                deliveryTag: eventArgs.DeliveryTag,
                                multiple: false,
                                requeue: false, // ставим в dlq сразу
                                cancellationToken: ct);
                        }
                        catch (Exception nackEx)
                        {
                            logger.LogError(nackEx, "Failed to NACK message");
                        }
                    }
                };

                await channel.BasicQosAsync(0, prefetchCount: 1, global: false, cancellationToken: ct);
                await channel.BasicConsumeAsync(queue: queueName.Required(), autoAck: false, consumer: consumer, cancellationToken: ct);
                logger.LogInformation("Starting consuming {QueueName}", queueName);
                return;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while setting up the consumer");

                if (attempt == maxRetryAttempts)
                {
                    logger.LogError("Maximum retry attempts reached. Consumer will not start.");
                    return;
                }

                var delayMs = Math.Pow(delayBetweenAttempts, attempt);
                logger.LogWarning("Retrying connection attempt {Attempt}/{MaxAttempts} in {Delay}ms...", attempt, maxRetryAttempts, delayMs);
                var delay = TimeSpan.FromMilliseconds(delayMs);
                await Task.Delay(delay, ct);
            }
        }
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        try
        {
            var channel = await channelFactory.Required().Value;
            if (channel.IsOpen)
            {
                await channel.CloseAsync(ct);
            }
            await channel.DisposeAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while closing channel");
        }
    }
}

