using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AutoIrrigationController : ControllerBase
{
    private readonly AutoIrrigationService _service;

    public AutoIrrigationController(AutoIrrigationService service)
    {
        _service = service;
    }

    [HttpPost("run")]
    public async Task<IActionResult> Run()
    {
        await _service.RunAsync();
        return Ok("Auto irrigation executed.");
    }
}
