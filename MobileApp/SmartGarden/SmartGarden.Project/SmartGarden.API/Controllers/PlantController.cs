using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using System.Security.Claims;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetAll()
        {
            var plants = await _plantService.GetAllAsync();
            return Ok(plants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetById(int id)
        {
            var plant = await _plantService.GetByIdAsync(id);
            if (plant == null) return NotFound();
            return Ok(plant);
        }

        [HttpPost]
        public async Task<ActionResult<Plant>> Create(CreatePlantDto dto)
        {
            var plant = await _plantService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = plant.PlantId }, plant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _plantService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
