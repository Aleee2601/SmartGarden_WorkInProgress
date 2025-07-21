using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGarden.API.Data;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly SmartGardenContext _context;

    public DashboardController(SmartGardenContext context)
    {
        _context = context;
    }

    // ✅ Endpoint 1: Planta cea mai udată
    [HttpGet("most-irrigated")]
    public async Task<IActionResult> GetMostIrrigatedPlant()
    {
        var result = await _context.IrrigationEvents
            .GroupBy(e => e.PlantId)
            .Select(g => new
            {
                PlantId = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        if (result == null)
            return NotFound("No irrigation data found.");

        var plant = await _context.Plants.FindAsync(result.PlantId);

        return Ok(new
        {
            plant?.Name,
            plant?.Type,
            TimesWatered = result.Count
        });
    }

    // ✅ Endpoint 2: Plante sub prag 30% (MoistureSoil < 300)
    [HttpGet("low-moisture-today")]
    public async Task<IActionResult> GetPlantsWithLowMoistureToday()
    {
        var cutoff = DateTime.UtcNow.AddDays(-1);

        var results = await _context.SensorReadings
            .Where(r => r.Timestamp >= cutoff && r.MoistureSoil < 300)
            .GroupBy(r => r.PlantId)
            .Select(g => new
            {
                PlantId = g.Key,
                Count = g.Count(),
                LastReading = g.Max(x => x.Timestamp)
            })
            .ToListAsync();

        var plants = await _context.Plants.ToListAsync();

        var response = results
            .Select(x => new
            {
                Plant = plants.FirstOrDefault(p => p.Id == x.PlantId)?.Name,
                x.Count,
                LastChecked = x.LastReading
            });

        return Ok(response);
    }
}
