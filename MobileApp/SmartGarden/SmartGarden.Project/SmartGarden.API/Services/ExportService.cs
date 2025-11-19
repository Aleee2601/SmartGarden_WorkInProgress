using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartGarden.Data.Persistence;
using SmartGarden.Core.Models;

namespace SmartGarden.API.Services;

/// <summary>
/// Service for exporting plant and sensor data
/// </summary>
public class ExportService : IExportService
{
    private readonly SmartGardenDbContext _dbContext;
    private readonly ILogger<ExportService> _logger;

    public ExportService(SmartGardenDbContext dbContext, ILogger<ExportService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Export sensor data to CSV
    /// </summary>
    public async Task<byte[]> ExportSensorDataToCsvAsync(int plantId, DateTime? startDate, DateTime? endDate)
    {
        var query = _dbContext.SensorReadings
            .Where(sr => sr.PlantId == plantId);

        if (startDate.HasValue)
            query = query.Where(sr => sr.ReadingTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(sr => sr.ReadingTime <= endDate.Value);

        var readings = await query
            .OrderBy(sr => sr.ReadingTime)
            .ToListAsync();

        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Timestamp,Soil Moisture (%),Air Temperature (°C),Air Humidity (%),Light Level (lux),Air Quality,Water Level (%)");

        // Data rows
        foreach (var reading in readings)
        {
            csv.AppendLine($"{reading.ReadingTime:yyyy-MM-dd HH:mm:ss}," +
                          $"{reading.SoilMoisture?.ToString() ?? ""}," +
                          $"{reading.AirTemperature?.ToString() ?? ""}," +
                          $"{reading.AirHumidity?.ToString() ?? ""}," +
                          $"{reading.LightLevel?.ToString() ?? ""}," +
                          $"{reading.AirQuality?.ToString() ?? ""}," +
                          $"{reading.WaterLevel?.ToString() ?? ""}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Export all plants data to CSV
    /// </summary>
    public async Task<byte[]> ExportAllPlantsToCsvAsync(int userId)
    {
        var plants = await _dbContext.Plants
            .Include(p => p.SensorReadings.OrderByDescending(sr => sr.ReadingTime).Take(1))
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .ToListAsync();

        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Plant Name,Species,Location,Auto Watering,Last Reading,Soil Moisture (%),Temperature (°C),Light Level (lux),Water Level (%)");

        // Data rows
        foreach (var plant in plants)
        {
            var lastReading = plant.SensorReadings.FirstOrDefault();
            csv.AppendLine($"\"{plant.Name}\"," +
                          $"\"{plant.Species}\"," +
                          $"\"{plant.Location ?? ""}\"," +
                          $"{plant.AutoWateringEnabled}," +
                          $"{lastReading?.ReadingTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never"}," +
                          $"{lastReading?.SoilMoisture?.ToString() ?? ""}," +
                          $"{lastReading?.AirTemperature?.ToString() ?? ""}," +
                          $"{lastReading?.LightLevel?.ToString() ?? ""}," +
                          $"{lastReading?.WaterLevel?.ToString() ?? ""}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    /// <summary>
    /// Generate PDF report for a plant (HTML-based, can be converted to PDF on client)
    /// </summary>
    public async Task<byte[]> GeneratePlantReportPdfAsync(int plantId, DateTime? startDate, DateTime? endDate)
    {
        var plant = await _dbContext.Plants
            .Include(p => p.User)
            .Include(p => p.Device)
            .FirstOrDefaultAsync(p => p.PlantId == plantId);

        if (plant == null)
            throw new Exception("Plant not found");

        var query = _dbContext.SensorReadings
            .Where(sr => sr.PlantId == plantId);

        if (startDate.HasValue)
            query = query.Where(sr => sr.ReadingTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(sr => sr.ReadingTime <= endDate.Value);

        var readings = await query
            .OrderByDescending(sr => sr.ReadingTime)
            .ToListAsync();

        // Calculate statistics
        var avgSoilMoisture = readings.Where(r => r.SoilMoisture.HasValue).Average(r => r.SoilMoisture) ?? 0;
        var avgTemperature = readings.Where(r => r.AirTemperature.HasValue).Average(r => r.AirTemperature) ?? 0;
        var avgHumidity = readings.Where(r => r.AirHumidity.HasValue).Average(r => r.AirHumidity) ?? 0;
        var avgLight = readings.Where(r => r.LightLevel.HasValue).Average(r => r.LightLevel) ?? 0;

        var html = GenerateHtmlReport(plant, readings, startDate, endDate,
            avgSoilMoisture, avgTemperature, avgHumidity, avgLight);

        return Encoding.UTF8.GetBytes(html);
    }

    /// <summary>
    /// Generate PDF summary for all plants
    /// </summary>
    public async Task<byte[]> GenerateUserSummaryPdfAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        var plants = await _dbContext.Plants
            .Include(p => p.SensorReadings.OrderByDescending(sr => sr.ReadingTime).Take(1))
            .Include(p => p.Device)
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .ToListAsync();

        var html = GenerateUserSummaryHtml(user, plants);

        return Encoding.UTF8.GetBytes(html);
    }

    #region HTML Generation

    private string GenerateHtmlReport(Plant plant, List<SensorReading> readings,
        DateTime? startDate, DateTime? endDate,
        double avgSoilMoisture, double avgTemperature, double avgHumidity, double avgLight)
    {
        var dateRange = $"{startDate?.ToString("yyyy-MM-dd") ?? "All time"} to {endDate?.ToString("yyyy-MM-dd") ?? "Now"}";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Plant Report - {plant.Name}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }}
        .header {{ background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 30px; text-align: center; border-radius: 10px; }}
        .stats {{ display: flex; justify-content: space-around; margin: 30px 0; }}
        .stat {{ text-align: center; padding: 20px; background: #f9fafb; border-radius: 8px; flex: 1; margin: 0 10px; }}
        .stat-value {{ font-size: 32px; font-weight: bold; color: #10b981; }}
        .stat-label {{ font-size: 14px; color: #6b7280; margin-top: 5px; }}
        .info {{ background: #eff6ff; padding: 20px; border-radius: 8px; margin: 20px 0; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th {{ background: #10b981; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #e5e7eb; }}
        tr:nth-child(even) {{ background: #f9fafb; }}
        .footer {{ text-align: center; color: #6b7280; margin-top: 40px; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🌱 Plant Report</h1>
        <h2>{plant.Name}</h2>
        <p>{plant.Species}</p>
    </div>

    <div class='info'>
        <p><strong>Report Period:</strong> {dateRange}</p>
        <p><strong>Location:</strong> {plant.Location ?? "Not specified"}</p>
        <p><strong>Auto Watering:</strong> {(plant.AutoWateringEnabled ? "Enabled" : "Disabled")}</p>
        <p><strong>Total Readings:</strong> {readings.Count}</p>
    </div>

    <h3>Average Values</h3>
    <div class='stats'>
        <div class='stat'>
            <div class='stat-value'>{avgSoilMoisture:F1}%</div>
            <div class='stat-label'>Soil Moisture</div>
        </div>
        <div class='stat'>
            <div class='stat-value'>{avgTemperature:F1}°C</div>
            <div class='stat-label'>Temperature</div>
        </div>
        <div class='stat'>
            <div class='stat-value'>{avgHumidity:F1}%</div>
            <div class='stat-label'>Humidity</div>
        </div>
        <div class='stat'>
            <div class='stat-value'>{avgLight:F0}</div>
            <div class='stat-label'>Light Level</div>
        </div>
    </div>

    <h3>Recent Sensor Readings</h3>
    <table>
        <thead>
            <tr>
                <th>Timestamp</th>
                <th>Soil Moisture</th>
                <th>Temperature</th>
                <th>Humidity</th>
                <th>Light Level</th>
                <th>Water Level</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", readings.Take(50).Select(r => $@"
            <tr>
                <td>{r.ReadingTime:yyyy-MM-dd HH:mm}</td>
                <td>{r.SoilMoisture?.ToString("F1") ?? "-"}%</td>
                <td>{r.AirTemperature?.ToString("F1") ?? "-"}°C</td>
                <td>{r.AirHumidity?.ToString("F1") ?? "-"}%</td>
                <td>{r.LightLevel?.ToString("F0") ?? "-"}</td>
                <td>{r.WaterLevel?.ToString("F0") ?? "-"}%</td>
            </tr>
            "))}
        </tbody>
    </table>

    <div class='footer'>
        <p>Generated by Bloomly SmartGarden on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        <p>© 2024 SmartGarden. All rights reserved.</p>
    </div>
</body>
</html>";
    }

    private string GenerateUserSummaryHtml(User user, List<Plant> plants)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Garden Summary Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }}
        .header {{ background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 30px; text-align: center; border-radius: 10px; }}
        .summary {{ background: #f9fafb; padding: 20px; border-radius: 8px; margin: 20px 0; }}
        .plant-card {{ background: white; padding: 20px; margin: 15px 0; border-radius: 8px; border-left: 4px solid #10b981; }}
        .stats {{ display: flex; justify-content: space-around; margin: 30px 0; }}
        .stat {{ text-align: center; padding: 20px; background: #f9fafb; border-radius: 8px; flex: 1; margin: 0 10px; }}
        .stat-value {{ font-size: 32px; font-weight: bold; color: #10b981; }}
        .stat-label {{ font-size: 14px; color: #6b7280; margin-top: 5px; }}
        .footer {{ text-align: center; color: #6b7280; margin-top: 40px; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🌱 Garden Summary Report</h1>
        <p>For: {user.Username ?? user.Name}</p>
        <p>{DateTime.Now:MMMM dd, yyyy}</p>
    </div>

    <div class='stats'>
        <div class='stat'>
            <div class='stat-value'>{plants.Count}</div>
            <div class='stat-label'>Total Plants</div>
        </div>
        <div class='stat'>
            <div class='stat-value'>{plants.Count(p => p.AutoWateringEnabled)}</div>
            <div class='stat-label'>Auto-Watering</div>
        </div>
        <div class='stat'>
            <div class='stat-value'>{plants.Count(p => p.Device != null)}</div>
            <div class='stat-label'>Monitored</div>
        </div>
    </div>

    <h2>Your Plants</h2>
    {string.Join("", plants.Select(p =>
    {
        var lastReading = p.SensorReadings.FirstOrDefault();
        return $@"
    <div class='plant-card'>
        <h3>{p.Name}</h3>
        <p><em>{p.Species}</em></p>
        <p><strong>Location:</strong> {p.Location ?? "Not specified"}</p>
        <p><strong>Auto Watering:</strong> {(p.AutoWateringEnabled ? "✅ Enabled" : "❌ Disabled")}</p>
        {(lastReading != null ? $@"
        <p><strong>Latest Reading:</strong> {lastReading.ReadingTime:yyyy-MM-dd HH:mm}</p>
        <p>Soil Moisture: {lastReading.SoilMoisture?.ToString("F1") ?? "-"}% |
           Temperature: {lastReading.AirTemperature?.ToString("F1") ?? "-"}°C |
           Light: {lastReading.LightLevel?.ToString("F0") ?? "-"} lux</p>
        " : "<p><strong>Status:</strong> No readings yet</p>")}
    </div>";
    }))}

    <div class='footer'>
        <p>Generated by Bloomly SmartGarden on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        <p>© 2024 SmartGarden. All rights reserved.</p>
    </div>
</body>
</html>";
    }

    #endregion
}
