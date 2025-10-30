namespace SmartGarden.Core.Models
{
    public class SystemLog
    {
        public int SystemLogId { get; set; }

        // Log details
        public string LogLevel { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }

        // Request context
        public int? UserId { get; set; }
        public string? Endpoint { get; set; }
        public string? Method { get; set; }
        public int? StatusCode { get; set; }
        public int? Duration { get; set; }

        // Timing
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public User? User { get; set; }
    }
}
