using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class WateringLog
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Mode { get; set; } = "manual"; // "manual" sau "auto"
        public int DurationSec { get; set; }

    }
}
