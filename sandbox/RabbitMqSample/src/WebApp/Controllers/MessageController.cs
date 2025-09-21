using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly RabbitMqService _rabbitMqService;

    public MessageController(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost("direct")]
    public IActionResult SendDirect([FromBody] MessageDto dto)
    {
        _rabbitMqService.PublishDirect(dto.Message, dto.RoutingKey);
        return Ok(new { Status = "Message sent to direct exchange" });
    }

    [HttpPost("fanout")]
    public IActionResult SendFanout([FromBody] MessageDto dto)
    {
        _rabbitMqService.PublishFanout(dto.Message);
        return Ok(new { Status = "Message sent to fanout exchange" });
    }

    [HttpPost("topic")]
    public IActionResult SendTopic([FromBody] MessageDto dto)
    {
        _rabbitMqService.PublishTopic(dto.Message, dto.RoutingKey);
        return Ok(new { Status = "Message sent to topic exchange" });
    }
}

public class MessageDto
{
    public string Message { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}