using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.DTOs
{
    public class CreatePlantDto
    {
        public string Name { get; set; } = null!;
        public double MinMoisture { get; set; }
        public double MaxMoisture { get; set; }
    }
}
