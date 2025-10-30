namespace SmartGarden.Core.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }

        // Entity tracking
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;

        // Change tracking
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        // Request metadata
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public User? User { get; set; }
    }
}
