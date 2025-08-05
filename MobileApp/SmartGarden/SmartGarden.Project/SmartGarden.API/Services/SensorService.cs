﻿using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data;

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
                Temperature = dto.Temperature,
                Humidity = dto.Humidity,
                SoilMoisture = dto.SoilMoisture,
                Timestamp = DateTime.UtcNow
            };

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();
            return reading;
        }

        public async Task<IEnumerable<SensorReading>> GetReadingsForPlantAsync(int plantId)
        {
            return await _context.SensorReadings
                .Where(r => r.PlantId == plantId)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }
    }
}
