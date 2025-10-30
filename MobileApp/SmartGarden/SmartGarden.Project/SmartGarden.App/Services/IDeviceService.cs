using SmartGarden.Core.DTOs;

namespace SmartGarden.App.Services
{
    public interface IDeviceService
    {
        Task<List<DeviceResponseDto>> GetUserDevicesAsync(int userId);
        Task<DeviceResponseDto?> GetDeviceByIdAsync(int deviceId);
        Task<DeviceResponseDto?> CreateDeviceAsync(CreateDeviceDto dto);
        Task<DeviceResponseDto?> UpdateDeviceAsync(int deviceId, UpdateDeviceDto dto);
        Task<bool> DeleteDeviceAsync(int deviceId);
        Task<List<DeviceResponseDto>> GetOfflineDevicesAsync(int thresholdMinutes = 30);
    }
}
