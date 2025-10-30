using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.App.Services
{
    public interface ISensorService
    {
        Task<bool> SubmitReadingAsync(CreateSensorReadingDto dto);
        Task<List<SensorReading>> GetPlantReadingsAsync(int plantId);
        Task<SensorReading?> GetLatestReadingAsync(int plantId);
    }
}
