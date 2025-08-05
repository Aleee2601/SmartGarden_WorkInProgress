using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.DTOs
{
    public class WateringLogDto
    {
        public DateTime Timestamp { get; set; }
        public string Mode { get; set; } = "manual";
    }
}
