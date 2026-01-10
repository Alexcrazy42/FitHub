using System.Diagnostics;
using System.Text;
using Confluent.Kafka;
using OpenTelemetry.Trace;
using ServiceA.Contracts;

namespace ServiceA.Services;

public class KafkaProducerService : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ActivitySource _activitySource;

    public KafkaProducerService(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
        _activitySource = new ActivitySource("ServiceA.KafkaProducer");
    }

    public async Task ProduceWeatherEventAsync(WeatherEvent weatherEvent)
    {
        using var activity = _activitySource.StartActivity("Produce Weather Event", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", "weather.events");
        activity?.SetTag("messaging.destination_kind", "topic");
        activity?.SetTag("city", weatherEvent.City);

        try
        {
            var message = new Message<string, string>
            {
                Key = weatherEvent.City,
                Value = System.Text.Json.JsonSerializer.Serialize(weatherEvent)
            };

            if (Activity.Current?.Context != default)
            {
                var traceparent = Activity.Current?.Id;
                message.Headers = new Headers();
                message.Headers.Add("traceparent", Encoding.UTF8.GetBytes(traceparent));
            }

            var deliveryResult = await _producer.ProduceAsync("weather.events", message);
            activity?.SetTag("messaging.kafka.message.offset", deliveryResult.Offset.Value);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(ex);
            throw;
        }
    }

    public void Dispose() => _producer?.Dispose();
}