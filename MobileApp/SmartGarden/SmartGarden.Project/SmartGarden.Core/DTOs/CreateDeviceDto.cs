namespace SmartGarden.Core.DTOs
{
    public class CreateDeviceDto
    {
        public int UserId { get; set; }
        public int? PlantId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceToken { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public int ReadingIntervalSec { get; set; } = 300;
    }
}
