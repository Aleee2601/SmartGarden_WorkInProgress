using System;

namespace SmartGarden.API.DTOs
{
    public class SensorReadingDTO
    {
        public Guid PlantId { get; set; }
        public float Temperature { get; set; }
        public float HumidityAir { get; set; }
        public int MoistureSoil { get; set; }
    }
}
