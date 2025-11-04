using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Timer;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IMetrics _metrics;
    private static readonly CounterOptions _requestsCounter = new CounterOptions
    {
        Name = "demo_requests_total",
        MeasurementUnit = Unit.Calls
    };

    private static readonly GaugeOptions _activeUsersGauge = new GaugeOptions
    {
        Name = "active_users",
        MeasurementUnit = Unit.Items
    };

    private static readonly TimerOptions _processingTimer = new TimerOptions
    {
        Name = "processing_time",
        MeasurementUnit = Unit.Requests
    };

    private static int _activeUsers = 0;
    private static readonly Random _rnd = new();

    public MetricsController(IMetrics metrics)
    {
        _metrics = metrics;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        _metrics.Measure.Counter.Increment(_requestsCounter);
        return Ok("pong");
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        _activeUsers++;
        _metrics.Measure.Gauge.SetValue(_activeUsersGauge, _activeUsers);
        _metrics.Measure.Counter.Increment(_requestsCounter);
        return Ok($"User logged in. Active users: {_activeUsers}");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (_activeUsers > 0)
            _activeUsers--;

        _metrics.Measure.Gauge.SetValue(_activeUsersGauge, _activeUsers);
        _metrics.Measure.Counter.Increment(_requestsCounter);
        return Ok($"User logged out. Active users: {_activeUsers}");
    }

    [HttpGet("process")]
    public IActionResult Process()
    {
        using (_metrics.Measure.Timer.Time(_processingTimer))
        {
            var delay = _rnd.Next(100, 1000);
            Thread.Sleep(delay);
        }

        _metrics.Measure.Counter.Increment(_requestsCounter);
        return Ok("Processed with simulated workload");
    }
}
