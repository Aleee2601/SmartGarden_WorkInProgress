namespace SmartGarden.Core.DTOs
{
    public class UpdateDeviceDto
    {
        public string? DeviceName { get; set; }
        public int? PlantId { get; set; }
        public string? IpAddress { get; set; }
        public string? FirmwareVersion { get; set; }
        public int? ReadingIntervalSec { get; set; }
        public bool? IsCalibrated { get; set; }
        public int? BatteryLevel { get; set; }
        public int? SignalStrength { get; set; }
    }
}
