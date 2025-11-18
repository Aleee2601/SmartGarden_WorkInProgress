using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class DeviceAuthService : IDeviceAuthService
    {
        private readonly SmartGardenDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeviceAuthService> _logger;

        // Security constants
        private const int MaxFailedAttempts = 5;
        private const int LockoutDurationMinutes = 30;
        private const int RateLimitPerHour = 120; // 120 requests per hour
        private const int TokenExpirationHours = 24;

        public DeviceAuthService(
            SmartGardenDbContext context,
            IConfiguration configuration,
            ILogger<DeviceAuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DeviceRegistrationResponse> RegisterDeviceAsync(DeviceRegistrationRequest request)
        {
            // Check if device already exists
            var existingDevice = await GetDeviceByMacAddressAsync(request.MacAddress);
            if (existingDevice != null)
            {
                throw new InvalidOperationException("Device already registered");
            }

            // Generate secure credentials
            var deviceId = Guid.NewGuid().ToString();
            var apiKey = GenerateSecureApiKey();
            var apiKeyHash = HashApiKey(apiKey);
            var refreshToken = GenerateSecureRefreshToken();

            // Create device entity
            var device = new Device
            {
                DeviceName = $"SmartGarden-{request.MacAddress.Substring(request.MacAddress.Length - 4)}",
                DeviceToken = string.Empty, // Will be set by JWT generation
                MacAddress = request.MacAddress,
                Model = request.Model,
                SerialNumber = request.SerialNumber,
                FirmwareVersion = request.FirmwareVersion,
                IsOnline = false,
                ReadingIntervalSec = 900, // 15 minutes default
                IsCalibrated = false,
                UserId = 0 // Will be set when approved
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            // Create device auth entity
            var deviceAuth = new DeviceAuth
            {
                DeviceId = device.DeviceId,
                ApiKeyHash = apiKeyHash,
                RefreshToken = refreshToken,
                TokenExpiry = DateTime.UtcNow.AddHours(TokenExpirationHours),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(30),
                IsApproved = false,
                RequestCount = 0,
                FailedAuthAttempts = 0,
                IsLocked = false
            };

            _context.DeviceAuths.Add(deviceAuth);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var deviceToken = GenerateDeviceJWT(device.DeviceId.ToString(), request.MacAddress);

            // Update device with token
            device.DeviceToken = deviceToken;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device registered successfully: {DeviceId}, MAC: {MacAddress}",
                device.DeviceId, request.MacAddress);

            return new DeviceRegistrationResponse
            {
                DeviceId = device.DeviceId.ToString(),
                DeviceToken = deviceToken,
                ApiKey = apiKey, // Send plain key ONLY on registration
                RefreshToken = refreshToken,
                ExpiresIn = (int)TimeSpan.FromHours(TokenExpirationHours).TotalSeconds,
                RequiresApproval = true
            };
        }

        public async Task<DeviceTokenRefreshResponse> RefreshDeviceTokenAsync(DeviceTokenRefreshRequest request)
        {
            var deviceId = int.Parse(request.DeviceId);
            var device = await _context.Devices
                .Include(d => d.DeviceAuth)
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

            if (device?.DeviceAuth == null)
            {
                throw new UnauthorizedAccessException("Device not found");
            }

            // Verify refresh token
            if (device.DeviceAuth.RefreshToken != request.RefreshToken)
            {
                await IncrementFailedAuthAsync(request.DeviceId);
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Check if refresh token expired
            if (device.DeviceAuth.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expired");
            }

            // Generate new JWT
            var newToken = GenerateDeviceJWT(request.DeviceId, device.MacAddress);
            device.DeviceToken = newToken;
            device.DeviceAuth.TokenExpiry = DateTime.UtcNow.AddHours(TokenExpirationHours);

            await _context.SaveChangesAsync();

            return new DeviceTokenRefreshResponse
            {
                DeviceToken = newToken,
                ExpiresIn = (int)TimeSpan.FromHours(TokenExpirationHours).TotalSeconds
            };
        }

        public async Task<bool> ApproveDeviceAsync(int userId, DeviceApprovalRequest request)
        {
            var device = await _context.Devices
                .Include(d => d.DeviceAuth)
                .FirstOrDefaultAsync(d => d.DeviceId == request.DeviceId);

            if (device?.DeviceAuth == null)
            {
                return false;
            }

            // Assign device to user
            device.UserId = userId;
            device.PlantId = request.PlantId;

            if (!string.IsNullOrEmpty(request.DeviceName))
            {
                device.DeviceName = request.DeviceName;
            }

            // Approve device
            device.DeviceAuth.IsApproved = true;
            device.DeviceAuth.ApprovedByUserId = userId.ToString();
            device.DeviceAuth.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Device approved: {DeviceId} by User: {UserId}", request.DeviceId, userId);

            return true;
        }

        public async Task<bool> VerifyApiKeyAsync(string deviceId, string apiKey)
        {
            var id = int.Parse(deviceId);
            var deviceAuth = await _context.DeviceAuths
                .FirstOrDefaultAsync(da => da.DeviceId == id);

            if (deviceAuth == null)
            {
                return false;
            }

            var hashedKey = HashApiKey(apiKey);
            return deviceAuth.ApiKeyHash == hashedKey;
        }

        public bool VerifySignature(string payload, string signature, string apiKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return signature.ToLower() == computedSignature;
        }

        public async Task<bool> UpdateHeartbeatAsync(DeviceHeartbeatRequest request)
        {
            var deviceId = int.Parse(request.DeviceId);
            var device = await _context.Devices.FindAsync(deviceId);

            if (device == null)
            {
                return false;
            }

            device.IsOnline = true;
            device.LastHeartbeat = DateTime.UtcNow;
            device.LastSeen = DateTime.UtcNow;
            device.BatteryLevel = request.BatteryLevel;
            device.SignalStrength = request.SignalStrength;

            if (!string.IsNullOrEmpty(request.FirmwareVersion))
            {
                device.FirmwareVersion = request.FirmwareVersion;
            }

            if (!string.IsNullOrEmpty(request.IpAddress))
            {
                device.IpAddress = request.IpAddress;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsDeviceLockedAsync(string deviceId)
        {
            var id = int.Parse(deviceId);
            var deviceAuth = await _context.DeviceAuths
                .FirstOrDefaultAsync(da => da.DeviceId == id);

            if (deviceAuth == null)
            {
                return false;
            }

            if (deviceAuth.IsLocked && deviceAuth.LockedUntil.HasValue)
            {
                // Check if lockout period has expired
                if (deviceAuth.LockedUntil.Value < DateTime.UtcNow)
                {
                    deviceAuth.IsLocked = false;
                    deviceAuth.LockedUntil = null;
                    deviceAuth.FailedAuthAttempts = 0;
                    await _context.SaveChangesAsync();
                    return false;
                }

                return true;
            }

            return false;
        }

        public async Task IncrementFailedAuthAsync(string deviceId)
        {
            var id = int.Parse(deviceId);
            var deviceAuth = await _context.DeviceAuths
                .FirstOrDefaultAsync(da => da.DeviceId == id);

            if (deviceAuth == null)
            {
                return;
            }

            deviceAuth.FailedAuthAttempts++;
            deviceAuth.LastFailedAuthAt = DateTime.UtcNow;

            // Lock device if max attempts exceeded
            if (deviceAuth.FailedAuthAttempts >= MaxFailedAttempts)
            {
                deviceAuth.IsLocked = true;
                deviceAuth.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);

                _logger.LogWarning("Device locked due to failed auth attempts: {DeviceId}", deviceId);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ResetFailedAuthAsync(string deviceId)
        {
            var id = int.Parse(deviceId);
            var deviceAuth = await _context.DeviceAuths
                .FirstOrDefaultAsync(da => da.DeviceId == id);

            if (deviceAuth != null)
            {
                deviceAuth.FailedAuthAttempts = 0;
                deviceAuth.LastFailedAuthAt = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckRateLimitAsync(string deviceId)
        {
            var id = int.Parse(deviceId);
            var deviceAuth = await _context.DeviceAuths
                .FirstOrDefaultAsync(da => da.DeviceId == id);

            if (deviceAuth == null)
            {
                return false;
            }

            // Reset rate limit counter if reset time passed
            if (deviceAuth.RateLimitResetAt.HasValue && deviceAuth.RateLimitResetAt.Value < DateTime.UtcNow)
            {
                deviceAuth.RequestCount = 0;
                deviceAuth.RateLimitResetAt = DateTime.UtcNow.AddHours(1);
            }
            else if (!deviceAuth.RateLimitResetAt.HasValue)
            {
                deviceAuth.RateLimitResetAt = DateTime.UtcNow.AddHours(1);
            }

            // Check if rate limit exceeded
            if (deviceAuth.RequestCount >= RateLimitPerHour)
            {
                _logger.LogWarning("Rate limit exceeded for device: {DeviceId}", deviceId);
                return false;
            }

            // Increment counter
            deviceAuth.RequestCount++;
            deviceAuth.LastRequestAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Device?> GetDeviceByMacAddressAsync(string macAddress)
        {
            return await _context.Devices
                .FirstOrDefaultAsync(d => d.MacAddress == macAddress);
        }

        public async Task<IEnumerable<Device>> GetPendingDevicesAsync(int userId)
        {
            return await _context.Devices
                .Include(d => d.DeviceAuth)
                .Where(d => d.UserId == userId && d.DeviceAuth != null && !d.DeviceAuth.IsApproved)
                .ToListAsync();
        }

        // Private helper methods

        private string GenerateDeviceJWT(string deviceId, string? macAddress)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, deviceId),
                new("device_id", deviceId),
                new("type", "device"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(macAddress))
            {
                claims.Add(new Claim("mac", macAddress));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:DeviceSecret"] ?? throw new InvalidOperationException("DeviceSecret not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(TokenExpirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateSecureApiKey()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string GenerateSecureRefreshToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string HashApiKey(string apiKey)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToBase64String(hash);
        }
    }
}
