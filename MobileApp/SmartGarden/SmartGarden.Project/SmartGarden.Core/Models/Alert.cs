using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int UserId { get; set; }
        public int? PlantId { get; set; }
        public int? DeviceId { get; set; }

        // Alert classification
        public AlertType AlertType { get; set; }
        public AlertSeverity Severity { get; set; }

        // Alert content
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }

        // Alert status
        public DateTime TriggeredAt { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public bool IsDismissed { get; set; } = false;
        public DateTime? DismissedAt { get; set; }
        public bool IsResolved { get; set; } = false;
        public DateTime? ResolvedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Plant? Plant { get; set; }
        public Device? Device { get; set; }
    }
}
