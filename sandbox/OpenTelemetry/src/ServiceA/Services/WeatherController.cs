using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceA.Contracts;
using ServiceA.Data;
using StackExchange.Redis;

namespace ServiceA.Services;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IDatabase _cache;
    private readonly KafkaProducerService _kafkaProducer;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(AppDbContext db, IDatabase cache, KafkaProducerService kafkaProducer, ILogger<WeatherController> logger)
    {
        _db = db;
        _cache = cache;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var cacheKey = $"weather:{city}";
        var cached = await _cache.StringGetAsync(cacheKey);

        if (!cached.IsNullOrEmpty)
        {
            _logger.LogInformation("Cache hit for {City}", city);
            return Ok(System.Text.Json.JsonSerializer.Deserialize<WeatherDto>(cached!));
        }

        var record = await _db.WeatherRecords
            .Where(w => w.City == city)
            .OrderByDescending(w => w.CreatedAt)
            .FirstOrDefaultAsync();

        if (record == null)
        {
            return NotFound();
        }

        var dto = new WeatherDto(record.City, record.TemperatureC, record.CreatedAt);
        
        await _cache.StringSetAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(dto), TimeSpan.FromMinutes(5));

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWeather([FromBody] CreateWeatherRequest request)
    {
        var record = new WeatherRecord
        {
            City = request.City,
            TemperatureC = request.TemperatureC,
            CreatedAt = DateTime.UtcNow
        };

        _db.WeatherRecords.Add(record);
        await _db.SaveChangesAsync();

        var weatherEvent = new WeatherEvent(request.City, request.TemperatureC, DateTime.UtcNow);
        await _kafkaProducer.ProduceWeatherEventAsync(weatherEvent);

        return CreatedAtAction(nameof(GetWeather), new { city = request.City }, record.Id);
    }
}

public record CreateWeatherRequest(string City, int TemperatureC);

public record WeatherDto(string City, int TemperatureC, DateTime Date);