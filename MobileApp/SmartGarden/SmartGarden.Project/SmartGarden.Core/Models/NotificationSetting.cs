using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class NotificationSetting
    {
        public int NotificationSettingId { get; set; }
        public int UserId { get; set; }

        // Channel configuration
        public NotificationChannel Channel { get; set; }
        public bool IsEnabled { get; set; } = true;

        // Filtering
        public string? AlertTypeFilter { get; set; }
        public int? MinSeverity { get; set; }

        // Quiet hours
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }

        // Endpoint (email, phone, push token, etc.)
        public string? Endpoint { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
