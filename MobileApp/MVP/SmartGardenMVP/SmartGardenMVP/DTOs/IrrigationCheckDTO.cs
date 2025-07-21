using System;

namespace SmartGarden.API.DTOs
{
    public class IrrigationCheckDTO
    {
        public Guid PlantId { get; set; }
        public int CurrentMoisture { get; set; }
    }
}
