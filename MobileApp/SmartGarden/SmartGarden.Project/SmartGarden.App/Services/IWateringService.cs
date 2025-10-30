namespace SmartGarden.App.Services
{
    public interface IWateringService
    {
        Task<bool> WaterPlantAsync(int plantId, string mode = "manual", int durationSec = 5);
    }
}
