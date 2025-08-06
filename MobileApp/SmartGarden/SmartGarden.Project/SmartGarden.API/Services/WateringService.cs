using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data;
using System.Net.Http;
using System.Text;

namespace SmartGarden.API.Services
{
    public class WateringService : IWateringService
    {
        private readonly SmartGardenDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WateringService(SmartGardenDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<WateringLog>> GetLogsAsync(int plantId)
        {
            return await _context.WateringLogs
                .Where(w => w.PlantId == plantId)
                .OrderByDescending(w => w.Timestamp)
                .ToListAsync();
        }

        public async Task<WateringLog> LogWateringAsync(int plantId, string mode)
        {
            var plantExists = await _context.Plants.AnyAsync(p => p.Id == plantId);
            if (!plantExists)
                throw new KeyNotFoundException($"Plant with ID {plantId} not found.");

            // Log in DB
            var log = new WateringLog
            {
                PlantId = plantId,
                Mode = mode,
                Timestamp = DateTime.UtcNow,
                DurationSec = 5 // Default duration
            };

            _context.WateringLogs.Add(log);
            await _context.SaveChangesAsync();

            // Send command to ESP
            string espIp = _configuration["ESP32:IpAddress"]; // ex. "192.168.0.150"
            string command = mode.ToLower() == "manual" ? "WATER ON" : "WATER AUTO";

            var content = new StringContent(command, Encoding.UTF8, "text/plain");
            try
            {
                var response = await _httpClient.PostAsync($"http://{espIp}/water", content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"ESP32 responded with error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send command to ESP: {ex.Message}");
            }

            return log;
        }
    }
}
