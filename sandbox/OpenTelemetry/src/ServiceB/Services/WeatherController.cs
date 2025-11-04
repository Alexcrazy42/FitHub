using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceB.Data;

namespace ServiceB.Services;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly AppDbContext _db;

    public WeatherController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await _db.WeatherLogs.ToListAsync();
        return Ok(logs);
    }
}