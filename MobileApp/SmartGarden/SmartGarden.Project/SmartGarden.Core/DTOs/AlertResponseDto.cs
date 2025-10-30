using SmartGarden.Core.Shared;

namespace SmartGarden.Core.DTOs
{
    public class AlertResponseDto
    {
        public int AlertId { get; set; }
        public int UserId { get; set; }
        public int? PlantId { get; set; }
        public int? DeviceId { get; set; }
        public AlertType AlertType { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime TriggeredAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsDismissed { get; set; }
        public DateTime? DismissedAt { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? PlantNickname { get; set; }
        public string? DeviceName { get; set; }
    }
}
