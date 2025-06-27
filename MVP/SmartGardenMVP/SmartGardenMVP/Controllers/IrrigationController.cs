using Microsoft.AspNetCore.Mvc;
using SmartGarden.API.Data;
using SmartGarden.API.DTOs;
using SmartGarden.API.Models;


namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IrrigationController : ControllerBase
    {
        private readonly SmartGardenContext _context;

        public IrrigationController(SmartGardenContext context)
        {
            _context = context;
        }

        // POST: /api/irrigation/check-moisture
        [HttpPost("check-moisture")]
        public async Task<ActionResult<bool>> CheckMoisture([FromBody] IrrigationCheckDTO dto)
        {
            var plant = await _context.Plants.FindAsync(dto.PlantId);
            if (plant == null) return NotFound();

            return dto.CurrentMoisture < plant.MoistureMin;
        }

        // POST: /api/irrigation/log
        [HttpPost("log")]
        public async Task<IActionResult> LogIrrigation([FromBody] IrrigationLogDTO dto)
        {
            var plant = await _context.Plants.FindAsync(dto.PlantId);
            if (plant == null) return NotFound();

            var ev = new IrrigationEvent
            {
                PlantId = dto.PlantId,
                MoistureLevel = dto.MoistureLevel,
                WasAutomatic = dto.WasAutomatic,
                Timestamp = DateTime.UtcNow
            };

            _context.IrrigationEvents.Add(ev);
            await _context.SaveChangesAsync();

            return Ok(ev);
        }
    }
}
