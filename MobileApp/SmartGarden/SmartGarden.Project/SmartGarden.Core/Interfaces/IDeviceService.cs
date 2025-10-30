using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.Core.Interfaces
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetAllAsync();
        Task<IEnumerable<Device>> GetByUserIdAsync(int userId);
        Task<Device?> GetByIdAsync(int deviceId);
        Task<Device?> GetByTokenAsync(string deviceToken);
        Task<Device> CreateAsync(CreateDeviceDto dto);
        Task<Device> UpdateAsync(int deviceId, UpdateDeviceDto dto);
        Task<bool> DeleteAsync(int deviceId);
        Task<bool> UpdateStatusAsync(int deviceId, bool isOnline);
        Task<bool> UpdateHeartbeatAsync(string deviceToken);
        Task<IEnumerable<Device>> GetOfflineDevicesAsync(TimeSpan threshold);
    }
}
