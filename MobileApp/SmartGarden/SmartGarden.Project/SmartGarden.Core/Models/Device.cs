namespace SmartGarden.Core.Models
{
    public class Device
    {
        public int DeviceId { get; set; }
        public int UserId { get; set; }
        public int? PlantId { get; set; }

        // Device identification
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceToken { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? IpAddress { get; set; }

        // Device configuration
        public string? FirmwareVersion { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }

        // Device status
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public int? BatteryLevel { get; set; }
        public int? SignalStrength { get; set; }

        // Device settings
        public int ReadingIntervalSec { get; set; } = 300;
        public bool IsCalibrated { get; set; }
        public DateTime? CalibrationDate { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Plant? Plant { get; set; }
        public ICollection<DeviceCommand> DeviceCommands { get; set; } = new List<DeviceCommand>();
    }
}
