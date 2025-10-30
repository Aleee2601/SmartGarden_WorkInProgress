using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class PlantHealth
    {
        public int PlantHealthId { get; set; }
        public int PlantId { get; set; }

        // Health calculation
        public DateTime CalculatedAt { get; set; }
        public PlantHealthStatus HealthStatus { get; set; }
        public double HealthScore { get; set; }

        // Component scores (0-100)
        public double? SoilMoistureScore { get; set; }
        public double? TemperatureScore { get; set; }
        public double? HumidityScore { get; set; }
        public double? LightScore { get; set; }
        public double? WateringScore { get; set; }

        // Analysis
        public string? Diagnosis { get; set; }
        public string? Recommendations { get; set; }

        // Navigation properties
        public Plant Plant { get; set; } = null!;
    }
}
