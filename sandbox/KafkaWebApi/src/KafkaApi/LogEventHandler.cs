using Messaging.Kafka;

namespace KafkaApi;

public class LogEventHandler : IMessageHandler<string, LogEvent>
{
    private readonly ILogger<LogEventHandler> _logger;

    public LogEventHandler(ILogger<LogEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(string key, LogEvent value, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{Level}] {Message} (Source={Source}, Key={Key}, Time={Time})",
            value.Level, value.Message, value.Source, key, value.Timestamp);

        return Task.CompletedTask;
    }
}