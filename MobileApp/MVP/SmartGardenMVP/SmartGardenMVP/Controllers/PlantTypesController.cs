using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGarden.API.Data;
using SmartGarden.API.DTOs;
using SmartGarden.API.Models;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantTypesController : ControllerBase
    {
        private readonly SmartGardenContext _context;

        public PlantTypesController(SmartGardenContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantType>>> GetAllTypes()
        {
            return await _context.PlantTypes.ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlantType([FromBody] PlantType type)
        {
            _context.PlantTypes.Add(type);
            await _context.SaveChangesAsync();
            return Ok(type);
        }

        [HttpPost("from-type")]
        public async Task<IActionResult> CreatePlantFromType([FromBody] CreatePlantFromTypeDTO dto)
        {
            var plantType = await _context.PlantTypes.FindAsync(dto.PlantTypeId);
            if (plantType == null)
                return NotFound("Plant type not found.");

            var plant = new Plant
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Type = plantType.Name,
                MoistureMin = plantType.DefaultMoistureMin,
                MoistureMax = plantType.DefaultMoistureMax,
                CreatedAt = DateTime.UtcNow
            };

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return Ok(plant);
        }

    }
}
