using System;

namespace SmartGarden.API.Models
{
    public class IrrigationEvent
    {
        public int Id { get; set; }
        public Guid PlantId { get; set; }
        public Plant Plant { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int MoistureLevel { get; set; }
        public bool WasAutomatic { get; set; }
    }
}
