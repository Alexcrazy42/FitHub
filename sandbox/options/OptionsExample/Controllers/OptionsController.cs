using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OptionsExample.Options;

namespace OptionsExample.Controllers;

[ApiController]
[Route("[controller]")]
public class OptionsController : ControllerBase
{
    private readonly SomeOptions someOptions;
    private readonly SomeOptions someOptionsFromSnap;
    private readonly SomeOptions someOptionsFromMonitor;

    public OptionsController(IOptions<SomeOptions> someOptions,
        IOptionsSnapshot<SomeOptions> optionsSnapshot,
        IOptionsMonitor<SomeOptions> optionsMonitor)
    {
        this.someOptions = someOptions.Value;
        this.someOptionsFromSnap = optionsSnapshot.Value;
        this.someOptionsFromMonitor = optionsMonitor.CurrentValue;
    }

    [HttpGet("options")]
    public SomeOptions GetOptions()
    {
        return someOptions;
    }

    [HttpGet("snap")]
    public SomeOptions GetOptionsFromSnap()
    {
        return someOptionsFromSnap;
    }

    [HttpGet("monitor")]
    public SomeOptions GetOptionsFromMonitor()
    {
        return someOptionsFromMonitor;
    }
}