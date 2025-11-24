namespace SmartGarden.Core.DTOs
{
    /// <summary>
    /// Request for historical sensor data
    /// </summary>
    public class HistoricalDataRequestDto
    {
        public int PlantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Aggregation interval: "hourly", "daily", "weekly"
        /// </summary>
        public string Interval { get; set; } = "hourly";
    }

    /// <summary>
    /// Aggregated sensor data point for charting
    /// </summary>
    public class SensorDataPointDto
    {
        public DateTime Timestamp { get; set; }
        public double AvgSoilMoisture { get; set; }
        public double MinSoilMoisture { get; set; }
        public double MaxSoilMoisture { get; set; }
        public double AvgAirTemp { get; set; }
        public double MinAirTemp { get; set; }
        public double MaxAirTemp { get; set; }
        public double AvgAirHumidity { get; set; }
        public double AvgLightLevel { get; set; }
        public double AvgWaterLevel { get; set; }
        public int ReadingCount { get; set; }
    }

    /// <summary>
    /// Response with aggregated historical data
    /// </summary>
    public class HistoricalDataResponseDto
    {
        public int PlantId { get; set; }
        public string? PlantName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Interval { get; set; } = "hourly";
        public List<SensorDataPointDto> DataPoints { get; set; } = new();
        public WateringEventDto[] WateringEvents { get; set; } = Array.Empty<WateringEventDto>();
        public PlantStatisticsDto? Statistics { get; set; }
    }

    /// <summary>
    /// Watering event marker for charts
    /// </summary>
    public class WateringEventDto
    {
        public long WateringId { get; set; }
        public DateTime Timestamp { get; set; }
        public int DurationSec { get; set; }
        public string Mode { get; set; } = "Manual";
    }

    /// <summary>
    /// Statistical summary for the period
    /// </summary>
    public class PlantStatisticsDto
    {
        public double AvgSoilMoisture { get; set; }
        public double MinSoilMoisture { get; set; }
        public double MaxSoilMoisture { get; set; }
        public double AvgAirTemp { get; set; }
        public double AvgLightLevel { get; set; }
        public int TotalReadings { get; set; }
        public int TotalWaterings { get; set; }
        public int DaysAboveThreshold { get; set; }
        public int DaysBelowThreshold { get; set; }
    }
}
