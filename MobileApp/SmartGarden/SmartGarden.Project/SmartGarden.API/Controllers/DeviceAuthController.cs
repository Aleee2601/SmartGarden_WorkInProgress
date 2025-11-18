using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using System.Security.Claims;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/device-auth")]
    public class DeviceAuthController : ControllerBase
    {
        private readonly IDeviceAuthService _deviceAuthService;
        private readonly ILogger<DeviceAuthController> _logger;

        public DeviceAuthController(
            IDeviceAuthService deviceAuthService,
            ILogger<DeviceAuthController> logger)
        {
            _deviceAuthService = deviceAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new ESP32 device (first-time setup)
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DeviceRegistrationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DeviceRegistrationResponse>> RegisterDevice(
            [FromBody] DeviceRegistrationRequest request)
        {
            try
            {
                // Validate MAC address format
                if (string.IsNullOrWhiteSpace(request.MacAddress) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(request.MacAddress,
                        @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$"))
                {
                    return BadRequest(new { error = "Invalid MAC address format" });
                }

                var response = await _deviceAuthService.RegisterDeviceAsync(request);

                _logger.LogInformation("Device registered: MAC={MacAddress}", request.MacAddress);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering device");
                return BadRequest(new { error = "Failed to register device" });
            }
        }

        /// <summary>
        /// Refresh device JWT token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DeviceTokenRefreshResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DeviceTokenRefreshResponse>> RefreshToken(
            [FromBody] DeviceTokenRefreshRequest request)
        {
            try
            {
                // Check if device is locked
                if (await _deviceAuthService.IsDeviceLockedAsync(request.DeviceId))
                {
                    return Unauthorized(new { error = "Device is temporarily locked due to failed authentication attempts" });
                }

                var response = await _deviceAuthService.RefreshDeviceTokenAsync(request);

                // Reset failed auth attempts on successful refresh
                await _deviceAuthService.ResetFailedAuthAsync(request.DeviceId);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token for device {DeviceId}", request.DeviceId);
                return BadRequest(new { error = "Failed to refresh token" });
            }
        }

        /// <summary>
        /// Approve device and assign to user/plant (requires user authentication)
        /// </summary>
        [HttpPost("approve")]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ApproveDevice([FromBody] DeviceApprovalRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { error = "Invalid user token" });
            }

            var success = await _deviceAuthService.ApproveDeviceAsync(userId, request);

            if (!success)
            {
                return NotFound(new { error = "Device not found" });
            }

            return Ok(new { message = "Device approved successfully" });
        }

        /// <summary>
        /// Get pending devices waiting for user approval
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(typeof(IEnumerable<DeviceResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetPendingDevices()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { error = "Invalid user token" });
            }

            var devices = await _deviceAuthService.GetPendingDevicesAsync(userId);

            var response = devices.Select(d => new DeviceResponseDto
            {
                DeviceId = d.DeviceId,
                DeviceName = d.DeviceName,
                MacAddress = d.MacAddress,
                Model = d.Model,
                FirmwareVersion = d.FirmwareVersion,
                IsOnline = d.IsOnline
            });

            return Ok(response);
        }

        /// <summary>
        /// Update device heartbeat (called by ESP32 every minute)
        /// </summary>
        [HttpPost("heartbeat")]
        [Authorize(Policy = "DeviceOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Heartbeat([FromBody] DeviceHeartbeatRequest request)
        {
            var deviceIdClaim = User.FindFirst("device_id")?.Value;

            if (string.IsNullOrEmpty(deviceIdClaim))
            {
                return Unauthorized(new { error = "Invalid device token" });
            }

            // Ensure device from token matches request
            if (deviceIdClaim != request.DeviceId)
            {
                return Unauthorized(new { error = "Device ID mismatch" });
            }

            // Check rate limit
            if (!await _deviceAuthService.CheckRateLimitAsync(request.DeviceId))
            {
                return StatusCode(StatusCodes.Status429TooManyRequests,
                    new { error = "Rate limit exceeded" });
            }

            var success = await _deviceAuthService.UpdateHeartbeatAsync(request);

            if (!success)
            {
                return NotFound(new { error = "Device not found" });
            }

            return Ok(new { message = "Heartbeat received", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Verify device API key (internal use)
        /// </summary>
        [HttpPost("verify-key")]
        [Authorize(Policy = "DeviceOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyApiKey([FromBody] VerifyApiKeyRequest request)
        {
            var deviceIdClaim = User.FindFirst("device_id")?.Value;

            if (string.IsNullOrEmpty(deviceIdClaim))
            {
                return Unauthorized(new { error = "Invalid device token" });
            }

            var isValid = await _deviceAuthService.VerifyApiKeyAsync(deviceIdClaim, request.ApiKey);

            if (!isValid)
            {
                await _deviceAuthService.IncrementFailedAuthAsync(deviceIdClaim);
                return Unauthorized(new { error = "Invalid API key" });
            }

            await _deviceAuthService.ResetFailedAuthAsync(deviceIdClaim);

            return Ok(new { valid = true });
        }
    }

    // Helper DTOs
    public class VerifyApiKeyRequest
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}
