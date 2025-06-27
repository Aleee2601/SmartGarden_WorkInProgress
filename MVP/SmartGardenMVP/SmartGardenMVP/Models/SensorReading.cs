using System;

namespace SmartGarden.API.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public Guid PlantId { get; set; }
        public Plant Plant { get; set; }
        public float Temperature { get; set; }
        public float HumidityAir { get; set; }
        public int MoistureSoil { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
