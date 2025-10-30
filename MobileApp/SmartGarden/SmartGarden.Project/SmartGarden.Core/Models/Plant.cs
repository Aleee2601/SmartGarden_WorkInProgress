using System;
using System.Collections.Generic;

namespace SmartGarden.Core.Models
{
    public class Plant
    {
        // Primary key
        public int PlantId { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int SpeciesId { get; set; }
        public int SoilTypeId { get; set; }

        // Plant properties (matches PlantConfiguration)
        public string? Nickname { get; set; }
        public string? RoomName { get; set; }
        public bool IsOutdoor { get; set; }
        public DateTime? DateAcquired { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Species Species { get; set; } = null!;
        public SoilType SoilType { get; set; } = null!;

        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
        public ICollection<WateringLog> WateringLogs { get; set; } = new List<WateringLog>();
    }
}
