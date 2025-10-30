namespace SmartGarden.Core.DTOs
{
    public class DeviceResponseDto
    {
        public int DeviceId { get; set; }
        public int UserId { get; set; }
        public int? PlantId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? IpAddress { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public int? BatteryLevel { get; set; }
        public int? SignalStrength { get; set; }
        public int ReadingIntervalSec { get; set; }
        public bool IsCalibrated { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public string? PlantNickname { get; set; }
    }
}
