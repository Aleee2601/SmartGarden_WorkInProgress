using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _plantService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plant = await _plantService.GetByIdAsync(id);
            return plant == null ? NotFound() : Ok(plant);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlantDto dto)
        {
            var plant = await _plantService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = plant.Id }, plant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _plantService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
