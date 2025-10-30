using System;
using System.Collections.Generic;

namespace SmartGarden.Core.Models
{
    public class Plant
    {
        // Primary key - EF Core maps this to "Id" column in database
        public int PlantId { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int SpeciesId { get; set; }
        public int SoilTypeId { get; set; }

        // Database properties from schema
        public string Name { get; set; } = string.Empty;
        public double MinMoisture { get; set; }
        public double MaxMoisture { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Species Species { get; set; } = null!;
        public SoilType SoilType { get; set; } = null!;

        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
        public ICollection<WateringLog> WateringLogs { get; set; } = new List<WateringLog>();
    }
}
