using System.Diagnostics;
using System.Text;
using Confluent.Kafka;
using OpenTelemetry.Trace;
using ServiceB.Contracts;
using ServiceB.Data;

namespace ServiceB.Services;

public class KafkaWeatherConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _bootstrapServers;
    private readonly ILogger<KafkaWeatherConsumer> _logger;
    private readonly ActivitySource _activitySource;

    public KafkaWeatherConsumer(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<KafkaWeatherConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        _logger = logger;
        _activitySource = new ActivitySource("ServiceB.KafkaConsumer");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "weather-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe("weather.events");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    string? traceparent = null;
                    if (consumeResult.Message.Headers != null)
                    {
                        var header = consumeResult.Message.Headers.GetLastBytes("traceparent");
                        if (header != null)
                        {
                            traceparent = Encoding.UTF8.GetString(header);
                        }
                    }
                    
                    ActivityContext? parentContext = null;
                    if (!string.IsNullOrEmpty(traceparent) && ActivityContext.TryParse(traceparent, null, out var context))
                    {
                        parentContext = context;
                    }
                    
                    using var activity = _activitySource.StartActivity(
                        "Consume Weather Event",
                        ActivityKind.Consumer,
                        parentContext ?? default(ActivityContext)
                    );
                    activity?.SetTag("messaging.system", "kafka");
                    activity?.SetTag("messaging.destination", "weather.events");
                    activity?.SetTag("messaging.destination_kind", "topic");
                    activity?.SetTag("messaging.kafka.message.offset", consumeResult.Offset.Value);
                    activity?.SetTag("city", consumeResult.Message.Key);

                    var weatherEvent = System.Text.Json.JsonSerializer.Deserialize<WeatherEvent>(consumeResult.Message.Value);

                    if (weatherEvent != null)
                    {
                        await ProcessWeatherEventAsync(weatherEvent, stoppingToken);
                        activity?.SetStatus(ActivityStatusCode.Ok);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message");
                    Activity.Current?.SetStatus(ActivityStatusCode.Error);
                    Activity.Current?.AddException(ex);
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessWeatherEventAsync(WeatherEvent weatherEvent, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

        db.WeatherLogs.Add(new WeatherLog
        {
            City = weatherEvent.City,
            TemperatureC = weatherEvent.TemperatureC,
            LoggedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync(ct);


        try
        {
            var client = httpClientFactory.CreateClient("ServiceA");
            var response = await client.GetAsync($"/weather/{weatherEvent.City}", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ServiceA returned {StatusCode} for {City}", response.StatusCode, weatherEvent.City);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call ServiceA for {City}", weatherEvent.City);
            throw;
        }
    }
}