using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Shared;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Controllers
{
    /// <summary>
    /// Analytics endpoints for historical sensor data and insights
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly SmartGardenDbContext _context;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            SmartGardenDbContext context,
            ILogger<AnalyticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/analytics/plant/{plantId}/historical
        /// Get aggregated historical sensor data for charts
        /// </summary>
        [HttpGet("plant/{plantId}/historical")]
        public async Task<ActionResult<HistoricalDataResponseDto>> GetHistoricalData(
            int plantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string interval = "hourly")
        {
            try
            {
                // Default to last 7 days if not specified
                var end = endDate ?? DateTime.UtcNow;
                var start = startDate ?? end.AddDays(-7);

                _logger.LogInformation(
                    "Fetching historical data for Plant {PlantId} from {Start} to {End} with {Interval} interval",
                    plantId, start, end, interval);

                // Verify plant exists
                var plant = await _context.Plants
                    .Include(p => p.PlantThresholds.Where(pt => pt.IsActive))
                    .FirstOrDefaultAsync(p => p.PlantId == plantId);

                if (plant == null)
                {
                    return NotFound($"Plant {plantId} not found");
                }

                // Get sensor readings
                var readings = await _context.SensorReadings
                    .Where(r => r.PlantId == plantId && r.CreatedAt >= start && r.CreatedAt <= end)
                    .OrderBy(r => r.CreatedAt)
                    .ToListAsync();

                // Get watering events
                var waterings = await _context.WateringLogs
                    .Where(w => w.PlantId == plantId && w.CreatedAt >= start && w.CreatedAt <= end)
                    .OrderBy(w => w.CreatedAt)
                    .Select(w => new WateringEventDto
                    {
                        WateringId = w.WateringId,
                        Timestamp = w.CreatedAt,
                        DurationSec = w.DurationSec,
                        Mode = w.Mode.ToString()
                    })
                    .ToArrayAsync();

                // Aggregate data based on interval
                var dataPoints = AggregateData(readings, interval, start, end);

                // Calculate statistics
                var threshold = plant.PlantThresholds.FirstOrDefault()?.MinSoilMoisture ?? 30.0;
                var statistics = CalculateStatistics(readings, waterings, threshold);

                var response = new HistoricalDataResponseDto
                {
                    PlantId = plantId,
                    PlantName = plant.Nickname,
                    StartDate = start,
                    EndDate = end,
                    Interval = interval,
                    DataPoints = dataPoints,
                    WateringEvents = waterings,
                    Statistics = statistics
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching historical data for Plant {PlantId}", plantId);
                return StatusCode(500, "Internal server error fetching historical data");
            }
        }

        /// <summary>
        /// GET /api/analytics/plant/{plantId}/summary
        /// Get quick summary statistics for dashboard
        /// </summary>
        [HttpGet("plant/{plantId}/summary")]
        public async Task<ActionResult<PlantStatisticsDto>> GetPlantSummary(
            int plantId,
            [FromQuery] int days = 7)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-days);

                var readings = await _context.SensorReadings
                    .Where(r => r.PlantId == plantId && r.CreatedAt >= startDate)
                    .ToListAsync();

                var waterings = await _context.WateringLogs
                    .Where(w => w.PlantId == plantId && w.CreatedAt >= startDate)
                    .ToListAsync();

                var plant = await _context.Plants
                    .Include(p => p.PlantThresholds.Where(pt => pt.IsActive))
                    .FirstOrDefaultAsync(p => p.PlantId == plantId);

                if (plant == null)
                {
                    return NotFound($"Plant {plantId} not found");
                }

                var threshold = plant.PlantThresholds.FirstOrDefault()?.MinSoilMoisture ?? 30.0;
                var statistics = CalculateStatistics(readings, waterings.Select(w => new WateringEventDto
                {
                    WateringId = w.WateringId,
                    Timestamp = w.CreatedAt,
                    DurationSec = w.DurationSec,
                    Mode = w.Mode.ToString()
                }).ToArray(), threshold);

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching summary for Plant {PlantId}", plantId);
                return StatusCode(500, "Internal server error fetching summary");
            }
        }

        /// <summary>
        /// Aggregates sensor readings based on time interval
        /// </summary>
        private List<SensorDataPointDto> AggregateData(
            List<Core.Models.SensorReading> readings,
            string interval,
            DateTime start,
            DateTime end)
        {
            if (!readings.Any())
            {
                return new List<SensorDataPointDto>();
            }

            // Determine grouping function
            Func<Core.Models.SensorReading, DateTime> groupByFunc = interval.ToLower() switch
            {
                "daily" => r => new DateTime(r.CreatedAt.Year, r.CreatedAt.Month, r.CreatedAt.Day),
                "weekly" => r => r.CreatedAt.Date.AddDays(-(int)r.CreatedAt.DayOfWeek),
                _ => r => new DateTime(r.CreatedAt.Year, r.CreatedAt.Month, r.CreatedAt.Day, r.CreatedAt.Hour, 0, 0) // hourly default
            };

            var grouped = readings
                .GroupBy(groupByFunc)
                .Select(g => new SensorDataPointDto
                {
                    Timestamp = g.Key,
                    AvgSoilMoisture = g.Average(r => r.SoilMoisture),
                    MinSoilMoisture = g.Min(r => r.SoilMoisture),
                    MaxSoilMoisture = g.Max(r => r.SoilMoisture),
                    AvgAirTemp = g.Average(r => r.AirTemp),
                    MinAirTemp = g.Min(r => r.AirTemp),
                    MaxAirTemp = g.Max(r => r.AirTemp),
                    AvgAirHumidity = g.Average(r => r.AirHumidity),
                    AvgLightLevel = g.Average(r => r.LightLevel),
                    AvgWaterLevel = g.Average(r => r.WaterLevel),
                    ReadingCount = g.Count()
                })
                .OrderBy(d => d.Timestamp)
                .ToList();

            return grouped;
        }

        /// <summary>
        /// Calculates statistical summary for the period
        /// </summary>
        private PlantStatisticsDto CalculateStatistics(
            List<Core.Models.SensorReading> readings,
            WateringEventDto[] waterings,
            double threshold)
        {
            if (!readings.Any())
            {
                return new PlantStatisticsDto
                {
                    TotalReadings = 0,
                    TotalWaterings = waterings.Length
                };
            }

            var daysAbove = readings
                .GroupBy(r => r.CreatedAt.Date)
                .Count(g => g.Average(r => r.SoilMoisture) >= threshold);

            var daysBelow = readings
                .GroupBy(r => r.CreatedAt.Date)
                .Count(g => g.Average(r => r.SoilMoisture) < threshold);

            return new PlantStatisticsDto
            {
                AvgSoilMoisture = readings.Average(r => r.SoilMoisture),
                MinSoilMoisture = readings.Min(r => r.SoilMoisture),
                MaxSoilMoisture = readings.Max(r => r.SoilMoisture),
                AvgAirTemp = readings.Average(r => r.AirTemp),
                AvgLightLevel = readings.Average(r => r.LightLevel),
                TotalReadings = readings.Count,
                TotalWaterings = waterings.Length,
                DaysAboveThreshold = daysAbove,
                DaysBelowThreshold = daysBelow
            };
        }
    }
}
