using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class PlantService : IPlantService
    {
        private readonly SmartGardenDbContext _context;

        public PlantService(SmartGardenDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Plant>> GetAllAsync()
            => await _context.Plants.ToListAsync();

        public async Task<Plant?> GetByIdAsync(int id)
            => await _context.Plants.FindAsync(id);

        public async Task<Plant> CreateAsync(CreatePlantDto dto)
        {
            var plant = new Plant
            {
                UserId = dto.UserId,
                SpeciesId = dto.SpeciesId,
                SoilTypeId = dto.SoilTypeId,
                Name = dto.Name,
                MinMoisture = dto.MinMoisture,
                MaxMoisture = dto.MaxMoisture
            };
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return plant;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null) return false;

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
