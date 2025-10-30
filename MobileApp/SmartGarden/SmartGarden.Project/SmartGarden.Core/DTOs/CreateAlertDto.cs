using SmartGarden.Core.Shared;

namespace SmartGarden.Core.DTOs
{
    public class CreateAlertDto
    {
        public int UserId { get; set; }
        public int? PlantId { get; set; }
        public int? DeviceId { get; set; }
        public AlertType AlertType { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
