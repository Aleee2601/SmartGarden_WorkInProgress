using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/plants/{plantId}/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpGet("readings")]
        [Authorize]
        public async Task<IActionResult> GetReadings(int plantId)
        {
            var readings = await _sensorService.GetReadingsForPlantAsync(plantId);
            return Ok(readings);
        }

        [HttpPost("readings")]
        [AllowAnonymous] // Allow ESP32 to post without auth
        public async Task<IActionResult> AddReading(int plantId, CreateSensorReadingDto dto)
        {
            var reading = await _sensorService.AddReadingAsync(plantId, dto);
            return CreatedAtAction(nameof(GetReadings), new { plantId = plantId }, reading);
        }
    }
}
