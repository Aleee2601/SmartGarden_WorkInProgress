using SmartGarden.App.Constants;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.App.Services
{
    public class SensorService : BaseApiService, ISensorService
    {
        public SensorService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<bool> SubmitReadingAsync(CreateSensorReadingDto dto)
        {
            return await PostAsync(ApiConstants.SensorReading, dto);
        }

        public async Task<List<SensorReading>> GetPlantReadingsAsync(int plantId)
        {
            var readings = await GetAsync<List<SensorReading>>(
                string.Format(ApiConstants.SensorByPlant, plantId));
            return readings ?? new List<SensorReading>();
        }

        public async Task<SensorReading?> GetLatestReadingAsync(int plantId)
        {
            return await GetAsync<SensorReading>(
                string.Format(ApiConstants.SensorLatest, plantId));
        }
    }
}
