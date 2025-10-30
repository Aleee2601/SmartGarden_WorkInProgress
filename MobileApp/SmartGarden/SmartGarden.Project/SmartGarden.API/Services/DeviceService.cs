using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly SmartGardenDbContext _context;

        public DeviceService(SmartGardenDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
            => await _context.Devices
                .Include(d => d.Plant)
                .ToListAsync();

        public async Task<IEnumerable<Device>> GetByUserIdAsync(int userId)
            => await _context.Devices
                .Where(d => d.UserId == userId)
                .Include(d => d.Plant)
                .ToListAsync();

        public async Task<Device?> GetByIdAsync(int deviceId)
            => await _context.Devices
                .Include(d => d.Plant)
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

        public async Task<Device?> GetByTokenAsync(string deviceToken)
            => await _context.Devices
                .Include(d => d.Plant)
                .FirstOrDefaultAsync(d => d.DeviceToken == deviceToken);

        public async Task<Device> CreateAsync(CreateDeviceDto dto)
        {
            var device = new Device
            {
                UserId = dto.UserId,
                PlantId = dto.PlantId,
                DeviceName = dto.DeviceName,
                DeviceToken = dto.DeviceToken,
                MacAddress = dto.MacAddress,
                Model = dto.Model,
                SerialNumber = dto.SerialNumber,
                ReadingIntervalSec = dto.ReadingIntervalSec,
                IsOnline = false,
                IsCalibrated = false
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<Device> UpdateAsync(int deviceId, UpdateDeviceDto dto)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
                throw new KeyNotFoundException($"Device {deviceId} not found");

            if (dto.DeviceName != null) device.DeviceName = dto.DeviceName;
            if (dto.PlantId.HasValue) device.PlantId = dto.PlantId;
            if (dto.IpAddress != null) device.IpAddress = dto.IpAddress;
            if (dto.FirmwareVersion != null) device.FirmwareVersion = dto.FirmwareVersion;
            if (dto.ReadingIntervalSec.HasValue) device.ReadingIntervalSec = dto.ReadingIntervalSec.Value;
            if (dto.IsCalibrated.HasValue) device.IsCalibrated = dto.IsCalibrated.Value;
            if (dto.BatteryLevel.HasValue) device.BatteryLevel = dto.BatteryLevel;
            if (dto.SignalStrength.HasValue) device.SignalStrength = dto.SignalStrength;

            if (device.IsCalibrated && device.CalibrationDate == null)
                device.CalibrationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<bool> DeleteAsync(int deviceId)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null) return false;

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int deviceId, bool isOnline)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null) return false;

            device.IsOnline = isOnline;
            device.LastSeen = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHeartbeatAsync(string deviceToken)
        {
            var device = await _context.Devices
                .FirstOrDefaultAsync(d => d.DeviceToken == deviceToken);

            if (device == null) return false;

            device.LastHeartbeat = DateTime.UtcNow;
            device.IsOnline = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Device>> GetOfflineDevicesAsync(TimeSpan threshold)
        {
            var cutoff = DateTime.UtcNow.Subtract(threshold);
            return await _context.Devices
                .Where(d => d.LastHeartbeat.HasValue && d.LastHeartbeat < cutoff)
                .Include(d => d.Plant)
                .ToListAsync();
        }
    }
}
