using SmartGarden.API.Data;
using SmartGarden.API.Models;
using Microsoft.EntityFrameworkCore;

public class AutoIrrigationService
{
    private readonly SmartGardenContext _context;

    public AutoIrrigationService(SmartGardenContext context)
    {
        _context = context;
    }

    public async Task RunAsync()
    {
        var plants = await _context.Plants
            .Where(p => p.AutoIrrigationEnabled)
            .ToListAsync();

        foreach (var plant in plants)
        {
            var latest = await _context.SensorReadings
                .Where(r => r.PlantId == plant.Id)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            if (latest == null || latest.MoistureSoil >= plant.MoistureMin)
                continue;

            // TRIMITERE COMANDĂ ESP - "WATER ON"
            SendCommandToESP(plant.Id, "WATER ON");

            await Task.Delay(5000); // așteaptă 5 secunde (udare)

            SendCommandToESP(plant.Id, "WATER OFF");

            await Task.Delay(TimeSpan.FromMinutes(2)); // așteaptă 2 minute

            // REFRESH citire senzor (aici poți citi direct dacă ai ESP conectat, sau iei cel mai recent)
            var refreshed = await _context.SensorReadings
                .Where(r => r.PlantId == plant.Id)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            if (refreshed != null && refreshed.MoistureSoil < plant.MoistureMin)
            {
                // udă din nou
                SendCommandToESP(plant.Id, "WATER ON");
                await Task.Delay(5000);
                SendCommandToESP(plant.Id, "WATER OFF");
            }
        }
    }

    private void SendCommandToESP(Guid plantId, string command)
    {
        // TODO: trimite HTTP POST către modulul ESP corespunzător
        // exemplu:
        // httpClient.PostAsync($"http://<ip>/water", new StringContent(command));
    }
}
