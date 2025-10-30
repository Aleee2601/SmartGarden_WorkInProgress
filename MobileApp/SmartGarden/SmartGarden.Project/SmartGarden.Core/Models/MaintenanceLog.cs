using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class MaintenanceLog
    {
        public int MaintenanceLogId { get; set; }
        public int PlantId { get; set; }

        // Maintenance details
        public MaintenanceType MaintenanceType { get; set; }
        public DateTime PerformedAt { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }

        // Cost tracking
        public decimal? Cost { get; set; }

        // Scheduling
        public DateTime? NextDueAt { get; set; }

        // Navigation properties
        public Plant Plant { get; set; } = null!;
    }
}
