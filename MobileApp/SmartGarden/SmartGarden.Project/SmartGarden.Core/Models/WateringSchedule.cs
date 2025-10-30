using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class WateringSchedule
    {
        public int WateringScheduleId { get; set; }
        public int PlantId { get; set; }

        // Schedule configuration
        public string ScheduleName { get; set; } = string.Empty;
        public ScheduleType ScheduleType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Schedule timing
        public TimeSpan? TimeOfDay { get; set; }
        public int? IntervalHours { get; set; }
        public string? DaysOfWeek { get; set; }

        // Watering settings
        public int DurationSec { get; set; } = 5;

        // Status
        public bool IsActive { get; set; } = true;
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }

        // Navigation properties
        public Plant Plant { get; set; } = null!;
    }
}
