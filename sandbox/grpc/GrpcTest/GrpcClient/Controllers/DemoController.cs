using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GrpcWebClient.Services;
using GrpcWebClient.Models;

namespace GrpcWebClient.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly IDemoClientService _demoService;

    public DemoController(IDemoClientService demoService)
    {
        _demoService = demoService;
    }

    /// <summary>
    /// Получить одну запись
    /// </summary>
    [HttpGet("data")]
    public async Task<ActionResult<DataResponseModel>> GetData(
        [FromQuery] int id = 1, 
        [FromQuery] string name = "Default")
    {
        var result = await _demoService.GetDataAsync(id, name);
        return Ok(result);
    }

    /// <summary>
    /// Получить список записей
    /// </summary>
    [HttpGet("data-list")]
    public async Task<ActionResult<DataListResponseModel>> GetDataList(
        [FromQuery] int id = 1,
        [FromQuery] string name = "Default")
    {
        var result = await _demoService.GetDataListAsync(id, name);
        return Ok(result);
    }

    /// <summary>
    /// Получить данные через stream
    /// </summary>
    [HttpGet("data-stream")]
    public async Task<ActionResult<List<DataResponseModel>>> GetDataStream(
        [FromQuery] int id = 1,
        [FromQuery] string name = "Default")
    {
        var result = await _demoService.GetDataStreamAsync(id, name);
        return Ok(result);
    }

    [HttpPost("stream")]
    public async Task<ActionResult> Stream()
    {
        await _demoService.BulkUpdate();
        return Ok();
    }
}