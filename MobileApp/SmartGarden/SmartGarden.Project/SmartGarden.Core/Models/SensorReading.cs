using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double SoilMoisture { get; set; }
    }
}
