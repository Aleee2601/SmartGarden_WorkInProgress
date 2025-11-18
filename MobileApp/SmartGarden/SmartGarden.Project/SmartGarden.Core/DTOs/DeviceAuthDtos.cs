namespace SmartGarden.Core.DTOs
{
    // Device Registration
    public class DeviceRegistrationRequest
    {
        public string MacAddress { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? FirmwareVersion { get; set; }
        public string? SerialNumber { get; set; }
    }

    public class DeviceRegistrationResponse
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceToken { get; set; } = string.Empty; // JWT for device
        public string ApiKey { get; set; } = string.Empty; // For HMAC signing
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } // Seconds until token expires
        public bool RequiresApproval { get; set; } = true;
    }

    // Device Token Refresh
    public class DeviceTokenRefreshRequest
    {
        public string DeviceId { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class DeviceTokenRefreshResponse
    {
        public string DeviceToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }

    // Device Approval (from user)
    public class DeviceApprovalRequest
    {
        public int DeviceId { get; set; }
        public int? PlantId { get; set; } // Assign device to plant
        public string? DeviceName { get; set; }
    }

    // Device Heartbeat
    public class DeviceHeartbeatRequest
    {
        public string DeviceId { get; set; } = string.Empty;
        public int? BatteryLevel { get; set; }
        public int? SignalStrength { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? IpAddress { get; set; }
    }

    // Sensor Reading with Security
    public class SecureSensorReadingRequest
    {
        public string DeviceId { get; set; } = string.Empty;
        public int? PlantId { get; set; }
        public float SoilMoisture { get; set; }
        public float AirTemperature { get; set; }
        public float AirHumidity { get; set; }
        public float? LightLevel { get; set; }
        public float? AirQuality { get; set; }
        public float? WaterLevel { get; set; }
        public long Timestamp { get; set; } // Unix timestamp
        public string Signature { get; set; } = string.Empty; // HMAC-SHA256 signature
    }
}
