using System;

namespace SmartGarden.API.Models
{
    public class AlertEvent
    {
        public int Id { get; set; }
        public Guid PlantId { get; set; }
        public Plant Plant { get; set; }

        public string Type { get; set; }      // "HighTemperature", "LowSoilMoisture"
        public string Message { get; set; }   // mesaj detaliat
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
