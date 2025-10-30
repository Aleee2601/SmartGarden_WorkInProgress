using System;
using System.Collections.Generic;

namespace SmartGarden.Core.Models
{
    public class Plant
    {
        // EF primary key (keeps existing DB column name)
        public int PlantId { get; set; }

        // Adapter property used by controllers/services
        public int Id
        {
            get => PlantId;
            set => PlantId = value;
        }

        public int UserId { get; set; }
        public int SpeciesId { get; set; }
        public int SoilTypeId { get; set; }

        // Keep existing fields but add the properties your services expect
        public string? Nickname { get; set; }
        public string? Name { get; set; }            // added to match CreatePlantDto
        public double MinMoisture { get; set; }     // added
        public double MaxMoisture { get; set; }     // added

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
