using FitHub.BankManager.Application.Mocks;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.BankManager.Web.Controllers;

[ApiController]
[Route("api/v1/bank/test")]
public class TestController : ControllerBase
{
    private readonly IMockTestService mockTestService;

    public TestController(IMockTestService mockTestService)
    {
        this.mockTestService = mockTestService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var test = await mockTestService.Test();

        return Ok(test);
    }
}
