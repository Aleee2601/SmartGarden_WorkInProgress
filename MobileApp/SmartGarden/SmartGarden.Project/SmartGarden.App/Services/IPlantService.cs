using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.App.Services
{
    public interface IPlantService
    {
        Task<List<Plant>> GetAllPlantsAsync();
        Task<Plant?> GetPlantByIdAsync(int plantId);
        Task<Plant?> CreatePlantAsync(CreatePlantDto dto);
        Task<bool> DeletePlantAsync(int plantId);
    }
}
