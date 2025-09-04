using System;

namespace SmartGarden.Core.Models
{
    public class SensorReading
    {
        public long ReadingId { get; set; }                      // necesar (eroarea ta)
        public int PlantId { get; set; }

        public double SoilMoisture { get; set; }                 // % sau raw
        public double AirTemp { get; set; }                      // °C
        public double AirHumidity { get; set; }                  // %
        public double LightLevel { get; set; }                   // lux sau %
        public double AirQuality { get; set; }                   // indice
        public double WaterLevel { get; set; }                   // % sau cm
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Plant Plant { get; set; } = null!;
    }
}
