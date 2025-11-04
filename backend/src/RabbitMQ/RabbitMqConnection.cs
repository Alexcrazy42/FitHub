using FitHub.RabbitMQ.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FitHub.RabbitMQ;

internal sealed class RabbitMqConnection<TOptions> : IRabbitMqConnection<TOptions>, IAsyncDisposable, IDisposable
    where TOptions : class, IRabbitMqOptions
{
    private readonly Lazy<Task<IConnection>> connectionFactory;
    private readonly Lazy<Task<IChannel>> channelFactory;
    private readonly TimeSpan connectionTimeout = TimeSpan.FromSeconds(5);
    private readonly TimeSpan channelTimeout = TimeSpan.FromSeconds(5);

    public RabbitMqConnection(IOptions<TOptions> options)
    {
        var factory = new ConnectionFactory
        {
            Endpoint = AmqpTcpEndpoint.Parse(options.Value.NodesRequired[Random.Shared.Next(options.Value.NodesRequired.Count)]),
            UserName = options.Value.UsernameRequired,
            Password = options.Value.PasswordRequired,
            Port = options.Value.Port,
            VirtualHost = options.Value.VirtualHostRequired,
            NetworkRecoveryInterval = options.Value.NetworkRecoveryInterval,
            RequestedConnectionTimeout = TimeSpan.FromSeconds(10)
        };

        connectionFactory = new Lazy<Task<IConnection>>(async () =>
        {
            using var cts = new CancellationTokenSource(connectionTimeout);
            try
            {
                return await factory.CreateConnectionAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Connection to RabbitMQ timed out after {connectionTimeout.TotalSeconds} seconds.");
            }
        });
        channelFactory = new Lazy<Task<IChannel>>(async () =>
        {
            using var cts = new CancellationTokenSource(channelTimeout);
            try
            {
                var connection = await connectionFactory.Value;

                return await connection.CreateChannelAsync(cancellationToken: cts.Token);
            }
            catch (OperationCanceledException)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    throw new TimeoutException($"Channel creation timed out after {channelTimeout.TotalSeconds} seconds.");
                }
                throw;
            }
        });
    }

    public Task<IChannel> CreateChannelAsync()
    {
        return channelFactory.Value;
    }


    public async ValueTask DisposeAsync()
    {
        if (channelFactory.IsValueCreated)
        {
            var channel = await channelFactory.Value;
            channel?.Dispose();
        }

        if (connectionFactory.IsValueCreated)
        {
            var connection = await connectionFactory.Value;
            connection?.Dispose();
        }
    }

    public void Dispose()
    {
        if (channelFactory.IsValueCreated)
        {
            var channel = channelFactory.Value.GetAwaiter().GetResult();
            channel?.Dispose();
        }

        if (connectionFactory.IsValueCreated)
        {
            var connection = connectionFactory.Value.GetAwaiter().GetResult();
            connection?.Dispose();
        }
    }
}
