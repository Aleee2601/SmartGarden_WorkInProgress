using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class Species
    {
        public int Id { get; set; }
        public string CommonName { get; set; } = null!;
        public string? ScientificName { get; set; }
        public int DefaultWaterFreqDays { get; set; }
        public double DefaultSoilMoistMin { get; set; }
        public double DefaultSoilMoistMax { get; set; }
        public double DefaultTempMin { get; set; }
        public double DefaultTempMax { get; set; }
        public double DefaultHumidMin { get; set; }
        public double DefaultHumidMax { get; set; }

        public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    }
}
