using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Models
{
    public class DeviceCommand
    {
        public int DeviceCommandId { get; set; }
        public int DeviceId { get; set; }

        // Command details
        public DeviceCommandType CommandType { get; set; }
        public string? Payload { get; set; }
        public CommandStatus Status { get; set; } = CommandStatus.Pending;

        // Timing
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Error handling
        public string? FailReason { get; set; }
        public int RetryCount { get; set; } = 0;

        // Navigation properties
        public Device Device { get; set; } = null!;
    }
}
