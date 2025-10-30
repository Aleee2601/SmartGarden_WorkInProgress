using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/plants/{plantId}/[controller]")]
    [Authorize]
    public class WateringController : ControllerBase
    {
        private readonly IWateringService _wateringService;

        public WateringController(IWateringService wateringService)
        {
            _wateringService = wateringService;
        }

        [HttpPost]
        public async Task<IActionResult> WaterPlant(int plantId, [FromQuery] string mode = "manual", [FromQuery] int durationSec = 5)
        {
            var log = await _wateringService.LogWateringAsync(plantId, mode, durationSec);

            return Ok(new
            {
                message = $"Plant {plantId} watered in {mode} mode for {durationSec} seconds.",
                log
            });
        }
    }
}
