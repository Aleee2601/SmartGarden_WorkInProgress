using SmartGarden.App.Constants;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.App.Services
{
    public class PlantService : BaseApiService, IPlantService
    {
        public PlantService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Plant>> GetAllPlantsAsync()
        {
            var plants = await GetAsync<List<Plant>>(ApiConstants.Plants);
            return plants ?? new List<Plant>();
        }

        public async Task<Plant?> GetPlantByIdAsync(int plantId)
        {
            return await GetAsync<Plant>(string.Format(ApiConstants.PlantById, plantId));
        }

        public async Task<Plant?> CreatePlantAsync(CreatePlantDto dto)
        {
            return await PostAsync<CreatePlantDto, Plant>(ApiConstants.Plants, dto);
        }

        public async Task<bool> DeletePlantAsync(int plantId)
        {
            return await DeleteAsync(string.Format(ApiConstants.PlantById, plantId));
        }
    }
}
