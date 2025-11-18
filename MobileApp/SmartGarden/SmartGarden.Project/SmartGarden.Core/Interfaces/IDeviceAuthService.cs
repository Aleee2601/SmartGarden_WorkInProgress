using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.Core.Interfaces
{
    public interface IDeviceAuthService
    {
        /// <summary>
        /// Register a new device and generate authentication credentials
        /// </summary>
        Task<DeviceRegistrationResponse> RegisterDeviceAsync(DeviceRegistrationRequest request);

        /// <summary>
        /// Refresh device JWT token using refresh token
        /// </summary>
        Task<DeviceTokenRefreshResponse> RefreshDeviceTokenAsync(DeviceTokenRefreshRequest request);

        /// <summary>
        /// Approve device and assign to user/plant
        /// </summary>
        Task<bool> ApproveDeviceAsync(int userId, DeviceApprovalRequest request);

        /// <summary>
        /// Verify device API key hash
        /// </summary>
        Task<bool> VerifyApiKeyAsync(string deviceId, string apiKey);

        /// <summary>
        /// Verify HMAC signature for sensor data
        /// </summary>
        bool VerifySignature(string payload, string signature, string apiKey);

        /// <summary>
        /// Update device heartbeat and status
        /// </summary>
        Task<bool> UpdateHeartbeatAsync(DeviceHeartbeatRequest request);

        /// <summary>
        /// Check if device is locked due to failed auth attempts
        /// </summary>
        Task<bool> IsDeviceLockedAsync(string deviceId);

        /// <summary>
        /// Increment failed auth attempts and lock if threshold exceeded
        /// </summary>
        Task IncrementFailedAuthAsync(string deviceId);

        /// <summary>
        /// Reset failed auth attempts on successful auth
        /// </summary>
        Task ResetFailedAuthAsync(string deviceId);

        /// <summary>
        /// Check rate limit for device
        /// </summary>
        Task<bool> CheckRateLimitAsync(string deviceId);

        /// <summary>
        /// Get device by MAC address
        /// </summary>
        Task<Device?> GetDeviceByMacAddressAsync(string macAddress);

        /// <summary>
        /// Get pending devices for user approval
        /// </summary>
        Task<IEnumerable<Device>> GetPendingDevicesAsync(int userId);
    }
}
