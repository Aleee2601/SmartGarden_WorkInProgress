using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGarden.API.Data;
using SmartGarden.API.DTOs;
using SmartGarden.API.Models;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly SmartGardenContext _context;

        public SensorReadingsController(SmartGardenContext context)
        {
            _context = context;
        }

        // GET: /api/sensorreadings/{plantId}/last-month
        [HttpGet("{plantId}/last-month")]
        public async Task<ActionResult<IEnumerable<SensorReading>>> GetLastMonthReadings(Guid plantId)
        {
            var oneMonthAgo = DateTime.UtcNow.AddDays(-30);

            var readings = await _context.SensorReadings
                .Where(r => r.PlantId == plantId && r.Timestamp >= oneMonthAgo)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();

            return Ok(readings);
        }

        // POST: /api/sensorreadings
        [HttpPost]
        public async Task<IActionResult> AddReading([FromBody] SensorReadingDTO dto)
        {
            var plant = await _context.Plants.FindAsync(dto.PlantId);
            if (plant == null)
                return NotFound("Plant not found.");

            var reading = new SensorReading
            {
                PlantId = dto.PlantId,
                Temperature = dto.Temperature,
                HumidityAir = dto.HumidityAir,
                MoistureSoil = dto.MoistureSoil,
                Timestamp = DateTime.UtcNow
            };

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reading saved successfully." });
        }

    }
}
