using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartGarden.API.Hubs;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;
using SmartGarden.Core.Shared;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Controllers
{
    /// <summary>
    /// Handles telemetry data from ESP32 devices and returns watering commands
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly SmartGardenDbContext _context;
        private readonly ILogger<TelemetryController> _logger;
        private readonly IHubContext<PlantHub> _hubContext;

        // Configuration constants
        private const int DEFAULT_WATERING_DURATION_SEC = 5;
        private const int DEFAULT_SLEEP_INTERVAL_SEC = 300; // 5 minutes
        private const double MIN_TANK_LEVEL_THRESHOLD = 5.0; // 5% minimum tank level

        public TelemetryController(
            SmartGardenDbContext context,
            ILogger<TelemetryController> logger,
            IHubContext<PlantHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        /// <summary>
        /// POST /api/telemetry
        /// Receives sensor data from ESP32 and returns watering command
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TelemetryResponseDto>> ReceiveTelemetry(
            [FromBody] TelemetryRequestDto request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                {
                    return BadRequest(new TelemetryResponseDto
                    {
                        Command = "ERROR",
                        Message = "Invalid telemetry data"
                    });
                }

                _logger.LogInformation(
                    "Received telemetry from Device {DeviceId}: SoilMoisture={SoilMoisture}%, TankLevel={TankLevel}%",
                    request.DeviceId, request.SoilMoisture, request.TankLevel);

                // 1. Find the device and associated plant
                var device = await _context.Devices
                    .Include(d => d.Plant)
                        .ThenInclude(p => p!.PlantThresholds.Where(pt => pt.IsActive))
                    .FirstOrDefaultAsync(d => d.DeviceId == request.DeviceId);

                if (device == null)
                {
                    _logger.LogWarning("Device {DeviceId} not found", request.DeviceId);
                    return NotFound(new TelemetryResponseDto
                    {
                        Command = "ERROR",
                        Message = $"Device {request.DeviceId} not registered"
                    });
                }

                // Update device last seen
                device.LastSeen = DateTime.UtcNow;
                device.IsOnline = true;
                await _context.SaveChangesAsync();

                // Check if device is assigned to a plant
                if (device.PlantId == null || device.Plant == null)
                {
                    _logger.LogInformation("Device {DeviceId} not assigned to any plant", request.DeviceId);

                    // Still record sensor data for unassigned device
                    await RecordSensorReadingAsync(null, request);

                    return Ok(new TelemetryResponseDto
                    {
                        Command = "SLEEP",
                        Message = "Device not assigned to plant",
                        NextCheckInSeconds = device.ReadingIntervalSec
                    });
                }

                var plant = device.Plant;
                var plantId = plant.PlantId;

                // 2. Record sensor reading to database and broadcast via SignalR
                await RecordSensorReadingAsync(plantId, request, plant.Nickname, false);

                // 3. Get plant thresholds
                var threshold = plant.PlantThresholds.FirstOrDefault(pt => pt.IsActive);
                if (threshold == null)
                {
                    _logger.LogWarning("Plant {PlantId} has no active thresholds configured", plantId);
                    return Ok(new TelemetryResponseDto
                    {
                        Command = "SLEEP",
                        Message = "No thresholds configured for this plant",
                        NextCheckInSeconds = device.ReadingIntervalSec
                    });
                }

                // 4. Determine if watering is needed
                var minMoistureThreshold = threshold.MinSoilMoisture ?? 30.0; // Default 30% if not set
                var needsWatering = request.SoilMoisture < minMoistureThreshold;
                var hasSufficientWater = request.TankLevel > MIN_TANK_LEVEL_THRESHOLD;

                _logger.LogInformation(
                    "Plant {PlantId} ({PlantName}): SoilMoisture={SoilMoisture}%, Threshold={Threshold}%, TankLevel={TankLevel}%, NeedsWatering={NeedsWatering}",
                    plantId, plant.Nickname ?? "Unnamed", request.SoilMoisture, minMoistureThreshold,
                    request.TankLevel, needsWatering);

                // 5. IF (Soil < Threshold AND Tank > MinLevel): WATER
                if (needsWatering && hasSufficientWater)
                {
                    var wateringDuration = CalculateWateringDuration(threshold);

                    // Create watering log entry
                    var wateringLog = new WateringLog
                    {
                        PlantId = plantId,
                        DurationSec = wateringDuration,
                        Mode = WateringMode.Auto,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.WateringLogs.AddAsync(wateringLog);
                    await _context.SaveChangesAsync();

                    // *** EF Core 10 OPTIMIZATION: Use ExecuteUpdateAsync for bulk update ***
                    // Update Plant.LastWateredDate without loading the entity
                    var now = DateTime.UtcNow;
                    await _context.Plants
                        .Where(p => p.PlantId == plantId)
                        .ExecuteUpdateAsync(setter => setter
                            .SetProperty(p => p.LastWateredDate, now));

                    _logger.LogInformation(
                        "WATERING Plant {PlantId} for {Duration} seconds (WateringLog {WateringId})",
                        plantId, wateringDuration, wateringLog.WateringId);

                    // Broadcast watering event via SignalR
                    await BroadcastPlantUpdateAsync(plantId, request, plant.Nickname, true);

                    return Ok(new TelemetryResponseDto
                    {
                        Command = "WATER",
                        Duration = wateringDuration,
                        Message = $"Soil moisture low ({request.SoilMoisture:F1}% < {minMoistureThreshold:F1}%)",
                        NextCheckInSeconds = device.ReadingIntervalSec
                    });
                }
                // 6. ELSE: SLEEP
                else
                {
                    var sleepReason = !needsWatering
                        ? $"Soil moisture adequate ({request.SoilMoisture:F1}% >= {minMoistureThreshold:F1}%)"
                        : $"Water tank low ({request.TankLevel:F1}% <= {MIN_TANK_LEVEL_THRESHOLD}%)";

                    if (!hasSufficientWater)
                    {
                        _logger.LogWarning(
                            "Plant {PlantId} needs watering but tank level too low: {TankLevel}%",
                            plantId, request.TankLevel);

                        // TODO: Create alert for low water tank
                    }

                    return Ok(new TelemetryResponseDto
                    {
                        Command = "SLEEP",
                        Message = sleepReason,
                        NextCheckInSeconds = device.ReadingIntervalSec
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing telemetry from Device {DeviceId}", request.DeviceId);
                return StatusCode(500, new TelemetryResponseDto
                {
                    Command = "ERROR",
                    Message = "Internal server error processing telemetry"
                });
            }
        }

        /// <summary>
        /// Records sensor reading to database using standard AddAsync
        /// </summary>
        private async Task RecordSensorReadingAsync(int? plantId, TelemetryRequestDto request,
            string? plantName, bool isWatering)
        {
            // Only record if plant is assigned
            if (plantId == null) return;

            var sensorReading = new SensorReading
            {
                PlantId = plantId.Value,
                SoilMoisture = request.SoilMoisture,
                WaterLevel = request.TankLevel,
                AirTemp = request.AirTemp ?? 0,
                AirHumidity = request.AirHumidity ?? 0,
                LightLevel = request.LightLevel ?? 0,
                AirQuality = request.AirQuality ?? 0,
                CreatedAt = DateTime.UtcNow
            };

            await _context.SensorReadings.AddAsync(sensorReading);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Recorded sensor reading {ReadingId} for Plant {PlantId}",
                sensorReading.ReadingId, plantId);

            // Broadcast real-time update via SignalR
            await BroadcastPlantUpdateAsync(plantId.Value, request, plantName, isWatering);
        }

        /// <summary>
        /// Broadcasts plant update to all connected SignalR clients
        /// </summary>
        private async Task BroadcastPlantUpdateAsync(int plantId, TelemetryRequestDto request,
            string? plantName, bool isWatering)
        {
            var update = new PlantUpdateDto
            {
                PlantId = plantId,
                PlantName = plantName,
                SoilMoisture = request.SoilMoisture,
                WaterLevel = request.TankLevel,
                AirTemp = request.AirTemp,
                AirHumidity = request.AirHumidity,
                LightLevel = request.LightLevel,
                AirQuality = request.AirQuality,
                Timestamp = DateTime.UtcNow,
                IsWatering = isWatering
            };

            // Broadcast to all clients (you can also use specific groups if needed)
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", update);

            // Alternative: Broadcast only to clients subscribed to this specific plant
            // await _hubContext.Clients.Group($"Plant_{plantId}").SendAsync("ReceiveUpdate", update);

            _logger.LogDebug("Broadcast plant update for Plant {PlantId} via SignalR", plantId);
        }

        /// <summary>
        /// Calculates watering duration based on plant thresholds
        /// </summary>
        private int CalculateWateringDuration(PlantThreshold threshold)
        {
            // Use configured watering interval or default
            // You can expand this logic to be more sophisticated
            if (threshold.IdealWateringIntervalHours.HasValue)
            {
                // More frequent watering intervals = shorter duration per watering
                var intervalHours = threshold.IdealWateringIntervalHours.Value;
                if (intervalHours <= 12)
                    return 3; // Short duration for frequent watering
                else if (intervalHours <= 48)
                    return 5; // Medium duration
                else
                    return 8; // Longer duration for infrequent watering
            }

            return DEFAULT_WATERING_DURATION_SEC;
        }

        /// <summary>
        /// GET /api/telemetry/health
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                Status = "healthy",
                Timestamp = DateTime.UtcNow,
                Message = "Telemetry controller is operational"
            });
        }
    }
}
