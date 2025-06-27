using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGarden.API.Data;
using SmartGarden.API.Models;
using SmartGarden.API.DTOs;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly SmartGardenContext _context;

        public PlantsController(SmartGardenContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            return await _context.Plants.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(Guid id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null) return NotFound();
            return plant;
        }

        [HttpPost]
        public async Task<ActionResult<Plant>> CreatePlant(PlantDTO dto)
        {
            var plant = new Plant
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Type = dto.Type,
                MoistureMin = dto.MoistureMin,
                MoistureMax = dto.MoistureMax
            };
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlant), new { id = plant.Id }, plant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(Guid id, PlantDTO dto)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null) return NotFound();

            plant.Name = dto.Name;
            plant.Type = dto.Type;
            plant.MoistureMin = dto.MoistureMin;
            plant.MoistureMax = dto.MoistureMax;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(Guid id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null) return NotFound();

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
