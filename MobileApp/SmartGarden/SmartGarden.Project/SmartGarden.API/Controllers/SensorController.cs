using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpPost("{plantId}")]
        public async Task<IActionResult> AddReading(int plantId, [FromBody] CreateSensorReadingDto dto)
        {
            var reading = await _sensorService.AddReadingAsync(plantId, dto);
            return Ok(reading);
        }

        [HttpGet("{plantId}")]
        public async Task<IActionResult> GetReadings(int plantId)
        {
            var readings = await _sensorService.GetReadingsForPlantAsync(plantId);
            return Ok(readings);
        }
    }
}
