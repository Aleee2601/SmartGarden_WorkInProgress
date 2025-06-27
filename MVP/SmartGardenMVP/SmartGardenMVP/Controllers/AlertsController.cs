using Microsoft.AspNetCore.Mvc;
using SmartGarden.API.Data;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly AlertService _alertService;
    private readonly SmartGardenContext _context;

    public AlertsController(AlertService alertService, SmartGardenContext context)
    {
        _alertService = alertService;
        _context = context;
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check()
    {
        await _alertService.CheckForCriticalAlertsAsync();
        return Ok("Checked alerts.");
    }

    [HttpGet("{plantId}")]
    public async Task<IActionResult> GetAlertsForPlant(Guid plantId)
    {
        var alerts = await _context.AlertEvents
            .Where(a => a.PlantId == plantId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        return Ok(alerts);
    }
}
