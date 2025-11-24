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
        private readonly IPlantInfoService _plantInfoService;

        public PlantController(
            IPlantService plantService,
            IPlantInfoService plantInfoService)
        {
            _plantService = plantService;
            _plantInfoService = plantInfoService;
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

        /// <summary>
        /// SMART SEARCH: Search for plant species using external plant database
        /// Returns auto-suggested moisture thresholds based on species characteristics
        /// </summary>
        /// <param name="q">Search query (e.g., "basil", "tomato", "snake plant")</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of matching plants with smart defaults</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PlantSearchResponseDto>>> SearchPlants(
            [FromQuery] string q,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Search query cannot be empty");
            }

            var results = await _plantInfoService.SearchPlantsAsync(q, cancellationToken);
            return Ok(results);
        }

        /// <summary>
        /// Get detailed information about a plant from external API
        /// </summary>
        [HttpGet("details/{externalApiId}")]
        public async Task<ActionResult<PlantSearchResponseDto>> GetPlantDetails(
            int externalApiId,
            CancellationToken cancellationToken)
        {
            var result = await _plantInfoService.GetPlantDetailsAsync(externalApiId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a plant from smart search result
        /// Accepts pre-filled data including auto-suggested moisture threshold
        /// </summary>
        [HttpPost("from-search")]
        public async Task<ActionResult<Plant>> CreateFromSearch(CreatePlantFromSearchDto dto)
        {
            // Map the search DTO to the standard create DTO
            var createDto = new CreatePlantDto
            {
                Nickname = dto.Nickname,
                SpeciesName = dto.SpeciesName,
                RoomName = dto.RoomName,
                IsOutdoor = dto.IsOutdoor,
                // Use the auto-suggested threshold from smart search
                MinMoisture = dto.MinMoistureThreshold,
                MaxMoisture = dto.MinMoistureThreshold + 20, // Auto-calculate max
                // Other fields can be set to defaults or from Species lookup
                SoilTypeId = 1, // Default soil type
                SpeciesId = 1   // Will be looked up based on SpeciesName
            };

            var plant = await _plantService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = plant.PlantId }, plant);
        }
    }
}
