using Microsoft.EntityFrameworkCore;
using SmartGarden.Data.Persistence;
using SmartGarden.Core.Models;

namespace SmartGarden.API.Services;

/// <summary>
/// Background service to monitor sensor data and send alerts
/// </summary>
public class AlertMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlertMonitorService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

    // Track last alert times to avoid spam (one alert per hour per plant/issue)
    private readonly Dictionary<string, DateTime> _lastAlertTimes = new();
    private readonly TimeSpan _alertCooldown = TimeSpan.FromHours(1);

    public AlertMonitorService(
        IServiceProvider serviceProvider,
        ILogger<AlertMonitorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Alert Monitor Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAlertsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Alert Monitor Service");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Alert Monitor Service stopped");
    }

    private async Task CheckAlertsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartGardenDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        // Get all plants with their latest sensor readings
        var plants = await dbContext.Plants
            .Include(p => p.User)
            .Include(p => p.SensorReadings.OrderByDescending(sr => sr.ReadingTime).Take(1))
            .Include(p => p.Device)
            .Where(p => !p.IsDeleted && p.User.EmailNotificationsEnabled)
            .ToListAsync();

        foreach (var plant in plants)
        {
            var latestReading = plant.SensorReadings.FirstOrDefault();
            if (latestReading == null) continue;

            var user = plant.User;

            // Check water level
            if (user.NotifyLowWater && latestReading.WaterLevel.HasValue && latestReading.WaterLevel < 20)
            {
                await SendAlertIfNeededAsync(
                    $"low-water-{plant.PlantId}",
                    async () => await emailService.SendLowWaterAlertAsync(user, plant, latestReading.WaterLevel.Value)
                );
            }

            // Check soil moisture
            if (user.NotifyLowSoilMoisture && latestReading.SoilMoisture.HasValue)
            {
                if (latestReading.SoilMoisture < plant.MinSoilMoisture)
                {
                    await SendAlertIfNeededAsync(
                        $"low-soil-{plant.PlantId}",
                        async () => await emailService.SendLowSoilMoistureAlertAsync(user, plant, latestReading.SoilMoisture.Value)
                    );
                }
            }

            // Check temperature
            if (user.NotifyExtremeTemperature && latestReading.AirTemperature.HasValue)
            {
                if (latestReading.AirTemperature > plant.MaxTemperature)
                {
                    await SendAlertIfNeededAsync(
                        $"high-temp-{plant.PlantId}",
                        async () => await emailService.SendExtremeTemperatureAlertAsync(user, plant, latestReading.AirTemperature.Value, true)
                    );
                }
                else if (latestReading.AirTemperature < plant.MinTemperature)
                {
                    await SendAlertIfNeededAsync(
                        $"low-temp-{plant.PlantId}",
                        async () => await emailService.SendExtremeTemperatureAlertAsync(user, plant, latestReading.AirTemperature.Value, false)
                    );
                }
            }

            // Check device offline
            if (user.NotifyDeviceOffline && plant.Device != null)
            {
                var lastHeartbeat = plant.Device.LastHeartbeat;
                if (lastHeartbeat.HasValue)
                {
                    var minutesSinceHeartbeat = (DateTime.UtcNow - lastHeartbeat.Value).TotalMinutes;
                    if (minutesSinceHeartbeat > 30) // 30 minutes offline
                    {
                        await SendAlertIfNeededAsync(
                            $"device-offline-{plant.Device.DeviceId}",
                            async () => await emailService.SendDeviceOfflineAlertAsync(user, plant.Device)
                        );
                    }
                }
            }
        }
    }

    private async Task SendAlertIfNeededAsync(string alertKey, Func<Task> sendAlertAction)
    {
        // Check cooldown
        if (_lastAlertTimes.TryGetValue(alertKey, out var lastAlertTime))
        {
            if (DateTime.UtcNow - lastAlertTime < _alertCooldown)
            {
                return; // Skip, still in cooldown
            }
        }

        // Send alert
        try
        {
            await sendAlertAction();
            _lastAlertTimes[alertKey] = DateTime.UtcNow;
            _logger.LogInformation($"Alert sent: {alertKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send alert: {alertKey}");
        }
    }
}
