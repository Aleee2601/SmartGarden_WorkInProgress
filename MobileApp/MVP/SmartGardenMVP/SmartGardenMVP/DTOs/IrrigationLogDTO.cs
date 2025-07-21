using System;

namespace SmartGarden.API.DTOs
{
    public class IrrigationLogDTO
    {
        public Guid PlantId { get; set; }
        public int MoistureLevel { get; set; }
        public bool WasAutomatic { get; set; }
    }
}
