using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double MinMoisture { get; set; }
        public double MaxMoisture { get; set; }

        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
        public ICollection<WateringLog> WateringLogs { get; set; } = new List<WateringLog>();
    }
}
