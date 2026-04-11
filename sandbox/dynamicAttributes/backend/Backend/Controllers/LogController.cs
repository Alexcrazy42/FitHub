using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly LogRepository _logRepository;

    public LogController(LogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Log>>> GetLogs()
    {
        var logs = await _logRepository.GetLogs();
        return Ok(logs);
    }

    [HttpGet("single")]
    public async Task<ActionResult<Log>> GetLog()
    {
        var log = await _logRepository.GetLog();
        return Ok(log);
    }

    [HttpGet("contains")]
    public async Task<ActionResult<IReadOnlyList<Log>>> GetLogs1()
    {
        var logs = await _logRepository.GetLogs1();
        return Ok(logs);
    }

    [HttpGet("exists-age")]
    public async Task<ActionResult<IReadOnlyList<Log>>> GetLogs2()
    {
        var logs = await _logRepository.GetLogs2();
        return Ok(logs);
    }

    [HttpGet("condition")]
    public async Task<ActionResult<IReadOnlyList<Log>>> GetLogs3()
    {
        var logs = await _logRepository.GetLogs3(5);
        return Ok(logs);
    }
}