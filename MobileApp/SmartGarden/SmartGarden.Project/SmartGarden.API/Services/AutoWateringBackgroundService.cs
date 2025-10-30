using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.Interfaces;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class AutoWateringBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoWateringBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public AutoWateringBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AutoWateringBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Auto Watering Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndWaterPlantsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking plants for auto watering");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Auto Watering Background Service stopped");
        }

        private async Task CheckAndWaterPlantsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SmartGardenDbContext>();
            var wateringService = scope.ServiceProvider.GetRequiredService<IWateringService>();

            // Get all users with auto watering enabled
            var usersWithAutoWatering = await dbContext.UserSettings
                .Where(us => us.AutoWateringEnabled)
                .Include(us => us.User)
                .ThenInclude(u => u.Plants)
                .ToListAsync(cancellationToken);

            _logger.LogInformation($"Found {usersWithAutoWatering.Count} users with auto watering enabled");

            foreach (var userSetting in usersWithAutoWatering)
            {
                var threshold = userSetting.SoilMoistThreshold;

                foreach (var plant in userSetting.User.Plants)
                {
                    try
                    {
                        // Get the most recent sensor reading for this plant
                        var latestReading = await dbContext.SensorReadings
                            .Where(sr => sr.PlantId == plant.PlantId)
                            .OrderByDescending(sr => sr.CreatedAt)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (latestReading == null)
                        {
                            _logger.LogWarning($"No sensor readings found for plant {plant.PlantId}");
                            continue;
                        }

                        // Check if reading is recent (within last 30 minutes)
                        var readingAge = DateTime.UtcNow - latestReading.CreatedAt;
                        if (readingAge > TimeSpan.FromMinutes(30))
                        {
                            _logger.LogWarning($"Latest reading for plant {plant.PlantId} is too old ({readingAge.TotalMinutes:F1} minutes)");
                            continue;
                        }

                        // Check if soil moisture is below threshold
                        if (latestReading.SoilMoisture < threshold)
                        {
                            // Check if we recently watered this plant (within last 2 hours)
                            var recentWatering = await dbContext.WateringLogs
                                .Where(wl => wl.PlantId == plant.PlantId)
                                .OrderByDescending(wl => wl.CreatedAt)
                                .FirstOrDefaultAsync(cancellationToken);

                            if (recentWatering != null)
                            {
                                var timeSinceWatering = DateTime.UtcNow - recentWatering.CreatedAt;
                                if (timeSinceWatering < TimeSpan.FromHours(2))
                                {
                                    _logger.LogInformation($"Plant {plant.PlantId} was watered {timeSinceWatering.TotalMinutes:F0} minutes ago, skipping");
                                    continue;
                                }
                            }

                            // Water the plant automatically
                            _logger.LogInformation($"Auto watering plant {plant.PlantId} (moisture: {latestReading.SoilMoisture:F1}%, threshold: {threshold}%)");

                            await wateringService.LogWateringAsync(plant.PlantId, "auto");

                            _logger.LogInformation($"Successfully watered plant {plant.PlantId}");
                        }
                        else
                        {
                            _logger.LogDebug($"Plant {plant.PlantId} moisture level OK ({latestReading.SoilMoisture:F1}% >= {threshold}%)");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing plant {plant.PlantId} for auto watering");
                    }
                }
            }
        }
    }
}
