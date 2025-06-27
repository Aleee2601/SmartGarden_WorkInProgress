using SmartGarden.API.Data;
using SmartGarden.API.Models;
using Microsoft.EntityFrameworkCore;

public class AlertService
{
    private readonly SmartGardenContext _context;

    public AlertService(SmartGardenContext context)
    {
        _context = context;
    }

    public async Task CheckForCriticalAlertsAsync()
    {
        var plants = await _context.Plants.ToListAsync();

        foreach (var plant in plants)
        {
            var latest = await _context.SensorReadings
                .Where(r => r.PlantId == plant.Id)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            if (latest == null) continue;

            if (latest.Temperature > 40)
            {
                await CreateAlert(plant.Id, "HighTemperature", $"Temperatura este {latest.Temperature}°C");
            }

            if (latest.MoistureSoil < 100) // ~10% dacă 1023 = 100%
            {
                await CreateAlert(plant.Id, "LowSoilMoisture", $"Umiditatea solului este {latest.MoistureSoil}");
            }
        }
    }

    private async Task CreateAlert(Guid plantId, string type, string message)
    {
        bool exists = await _context.AlertEvents
            .AnyAsync(a => a.PlantId == plantId && a.Type == type && a.Timestamp > DateTime.UtcNow.AddHours(-1));

        if (exists) return; // evităm duplicate inutile într-o oră

        var alert = new AlertEvent
        {
            PlantId = plantId,
            Type = type,
            Message = message
        };

        _context.AlertEvents.Add(alert);
        await _context.SaveChangesAsync();
    }
}
