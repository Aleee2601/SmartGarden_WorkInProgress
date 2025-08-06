using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class SoilType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int RecWaterDurSec { get; set; }
        public int PauseBetweenWaterMin { get; set; }

        public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    }
}
