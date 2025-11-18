namespace SmartGarden.Core.Models
{
    /// <summary>
    /// Device authentication and security credentials
    /// Stores encrypted tokens and API keys for secure ESP32 communication
    /// </summary>
    public class DeviceAuth
    {
        public int DeviceAuthId { get; set; }
        public int DeviceId { get; set; }

        // Security credentials
        public string ApiKeyHash { get; set; } = string.Empty; // SHA256 hash of API key
        public string RefreshToken { get; set; } = string.Empty; // For token refresh
        public DateTime? TokenExpiry { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // Security metadata
        public bool IsApproved { get; set; } = false; // User must approve device
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }

        // Certificate pinning
        public string? CertificateFingerprint { get; set; }

        // Rate limiting
        public int RequestCount { get; set; } = 0;
        public DateTime? LastRequestAt { get; set; }
        public DateTime? RateLimitResetAt { get; set; }

        // Security events
        public int FailedAuthAttempts { get; set; } = 0;
        public DateTime? LastFailedAuthAt { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime? LockedUntil { get; set; }

        // Navigation
        public Device Device { get; set; } = null!;
    }
}
