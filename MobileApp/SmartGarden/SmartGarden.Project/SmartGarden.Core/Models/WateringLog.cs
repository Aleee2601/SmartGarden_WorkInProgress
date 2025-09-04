using System;
using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class WateringLog
    {
        public long WateringId { get; set; }                     // necesar (eroarea ta)
        public int PlantId { get; set; }

        public int DurationSec { get; set; }
        public WateringMode Mode { get; set; } = WateringMode.Manual;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Plant Plant { get; set; } = null!;
    }
}
