using Messaging.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace KafkaApi.Controllers;

[ApiController]
public class OrderController : ControllerBase
{
    private readonly IKafkaProducer<string, OrderCreatedEvent> _producer;
    private readonly IKafkaProducer<string, LogEvent> _producer1;

    public OrderController(IKafkaProducer<string, OrderCreatedEvent> producer,
        IKafkaProducer<string, LogEvent> producer1)
    {
        _producer = producer;
        _producer1 = producer1;
    }

    [HttpPost("/order")]
    public async Task<IActionResult> Create()
    {
        var guid = Guid.NewGuid();
        await _producer.ProduceAsync(
            "orders",
            guid.ToString(),
            new OrderCreatedEvent { OrderId = guid.ToString() });

        return Ok();
    }

    [HttpPost("log")]
    public async Task<IActionResult> CreateLog()
    {
        var guid = Guid.NewGuid();
        var log = new LogEvent()
        {
            Source = guid.ToString(),
            Message = $"message {guid.ToString()}"
        };
        await _producer1.ProduceAsync("logs", 
            guid.ToString(), 
            log);

        return Ok();
    }
}