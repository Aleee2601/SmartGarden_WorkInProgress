namespace SmartGarden.Core.Models
{
    public class PlantThreshold
    {
        public int PlantThresholdId { get; set; }
        public int PlantId { get; set; }

        // Soil moisture thresholds (percentage)
        public double? MinSoilMoisture { get; set; }
        public double? MaxSoilMoisture { get; set; }

        // Air temperature thresholds (Celsius)
        public double? MinAirTemp { get; set; }
        public double? MaxAirTemp { get; set; }

        // Air humidity thresholds (percentage)
        public double? MinAirHumidity { get; set; }
        public double? MaxAirHumidity { get; set; }

        // Light level thresholds (lux)
        public double? MinLightLevel { get; set; }
        public double? MaxLightLevel { get; set; }

        // Watering settings
        public int? IdealWateringIntervalHours { get; set; }

        // Status
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Plant Plant { get; set; } = null!;
    }
}
