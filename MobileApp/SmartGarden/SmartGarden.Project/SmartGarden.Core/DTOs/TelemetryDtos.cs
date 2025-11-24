using System.ComponentModel.DataAnnotations;

namespace SmartGarden.Core.DTOs
{
    /// <summary>
    /// Telemetry data sent from ESP32 device
    /// </summary>
    public class TelemetryRequestDto
    {
        [Required]
        public int DeviceId { get; set; }

        [Required]
        [Range(0, 100)]
        public double SoilMoisture { get; set; }

        [Required]
        [Range(0, 100)]
        public double TankLevel { get; set; }

        // Optional additional sensor data
        public double? AirTemp { get; set; }
        public double? AirHumidity { get; set; }
        public double? LightLevel { get; set; }
        public double? AirQuality { get; set; }
    }

    /// <summary>
    /// Command response to ESP32 device
    /// </summary>
    public class TelemetryResponseDto
    {
        /// <summary>
        /// Command to execute: "WATER", "SLEEP", "ERROR"
        /// </summary>
        public string Command { get; set; } = "SLEEP";

        /// <summary>
        /// Water duration in seconds (only for WATER command)
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Additional message or error details
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Next check interval in seconds (for SLEEP command)
        /// </summary>
        public int? NextCheckInSeconds { get; set; }
    }

    /// <summary>
    /// Real-time sensor update broadcast via SignalR
    /// </summary>
    public class PlantUpdateDto
    {
        public int PlantId { get; set; }
        public string? PlantName { get; set; }
        public double SoilMoisture { get; set; }
        public double WaterLevel { get; set; }
        public double? AirTemp { get; set; }
        public double? AirHumidity { get; set; }
        public double? LightLevel { get; set; }
        public double? AirQuality { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsWatering { get; set; }
    }
}
