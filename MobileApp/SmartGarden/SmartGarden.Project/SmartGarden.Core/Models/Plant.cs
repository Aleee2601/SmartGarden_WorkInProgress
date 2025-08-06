using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class Plant
    {
        public int PlantId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int SpeciesId { get; set; }
        public Species Species { get; set; } = null!;
        public int SoilTypeId { get; set; }
        public SoilType SoilType { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public bool IsOutdoor { get; set; }
        public DateTime DateAcquired { get; set; }

        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
        public ICollection<WateringLog> WateringLogs { get; set; } = new List<WateringLog>();
    }

}
