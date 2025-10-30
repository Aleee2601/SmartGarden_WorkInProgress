using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class SensorService : ISensorService
    {
        private readonly SmartGardenDbContext _context;

        public SensorService(SmartGardenDbContext context)
        {
            _context = context;
        }

        public async Task<SensorReading> AddReadingAsync(int plantId, CreateSensorReadingDto dto)
        {
            var reading = new SensorReading
            {
                PlantId = plantId,
                AirTemp = dto.Temperature,
                AirHumidity = dto.Humidity,
                SoilMoisture = dto.SoilMoisture,
                LightLevel = 0, // Default value, can be updated later
                AirQuality = 0, // Default value, can be updated later
                WaterLevel = 0, // Default value, can be updated later
                CreatedAt = DateTime.UtcNow
            };

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();
            return reading;
        }

        public async Task<IEnumerable<SensorReading>> GetReadingsForPlantAsync(int plantId)
        {
            return await _context.SensorReadings
                .Where(r => r.PlantId == plantId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
