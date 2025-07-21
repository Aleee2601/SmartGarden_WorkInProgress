using SmartGarden.API.Data;     // pentru SmartGardenContext
using SmartGarden.API.Models;   // pentru IrrigationEvent
using System;                  
public class PlantService
{
    private readonly SmartGardenContext _context;

    public PlantService(SmartGardenContext context)
    {
        _context = context;
    }

    public async Task<bool> ShouldWaterAsync(Guid plantId, int currentSoilMoisture)
    {
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant == null) return false;

        return currentSoilMoisture < plant.MoistureMin;
    }

    public async Task LogIrrigationAsync(Guid plantId, int moisture, bool automatic)
    {
        _context.IrrigationEvents.Add(new IrrigationEvent
        {
            PlantId = plantId,
            MoistureLevel = moisture,
            WasAutomatic = automatic,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}
