using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.DTOs
{
    public class CreatePlantDto
    {
        public int UserId { get; set; }
        public int SpeciesId { get; set; }
        public int SoilTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double MinMoisture { get; set; }
        public double MaxMoisture { get; set; }
    }
}
