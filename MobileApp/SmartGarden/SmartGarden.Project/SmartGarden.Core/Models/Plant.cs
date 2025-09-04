using System;
using System.Collections.Generic;

namespace SmartGarden.Core.Models
{
    public class Plant
    {
        public int PlantId { get; set; }
        public int UserId { get; set; }
        public int SpeciesId { get; set; }
        public int SoilTypeId { get; set; }

        public string? Nickname { get; set; }                    // necesar (eroarea ta)
        public string? RoomName { get; set; }
        public bool IsOutdoor { get; set; }
        public DateTime? DateAcquired { get; set; }

        public User User { get; set; } = null!;
        public Species Species { get; set; } = null!;
        public SoilType SoilType { get; set; } = null!;

        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
        public ICollection<WateringLog> WateringLogs { get; set; } = new List<WateringLog>();
    }
}
