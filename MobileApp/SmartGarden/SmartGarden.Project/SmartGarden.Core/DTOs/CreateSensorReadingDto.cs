using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.DTOs
{
    public class CreateSensorReadingDto
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double SoilMoisture { get; set; }
    }
}
