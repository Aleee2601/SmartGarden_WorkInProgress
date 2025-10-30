using SmartGarden.App.Constants;

namespace SmartGarden.App.Services
{
    public class WateringService : BaseApiService, IWateringService
    {
        public WateringService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<bool> WaterPlantAsync(int plantId, string mode = "manual", int durationSec = 5)
        {
            var endpoint = string.Format(ApiConstants.WaterPlant, plantId) +
                $"?mode={mode}&durationSec={durationSec}";
            return await PostAsync(endpoint, new { });
        }
    }
}
