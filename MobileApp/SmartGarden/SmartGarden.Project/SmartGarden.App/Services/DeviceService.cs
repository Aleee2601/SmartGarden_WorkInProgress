using SmartGarden.App.Constants;
using SmartGarden.Core.DTOs;

namespace SmartGarden.App.Services
{
    public class DeviceService : BaseApiService, IDeviceService
    {
        public DeviceService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<DeviceResponseDto>> GetUserDevicesAsync(int userId)
        {
            var devices = await GetAsync<List<DeviceResponseDto>>(
                string.Format(ApiConstants.DevicesByUser, userId));
            return devices ?? new List<DeviceResponseDto>();
        }

        public async Task<DeviceResponseDto?> GetDeviceByIdAsync(int deviceId)
        {
            return await GetAsync<DeviceResponseDto>(
                string.Format(ApiConstants.DeviceById, deviceId));
        }

        public async Task<DeviceResponseDto?> CreateDeviceAsync(CreateDeviceDto dto)
        {
            return await PostAsync<CreateDeviceDto, DeviceResponseDto>(ApiConstants.Devices, dto);
        }

        public async Task<DeviceResponseDto?> UpdateDeviceAsync(int deviceId, UpdateDeviceDto dto)
        {
            return await PutAsync<UpdateDeviceDto, DeviceResponseDto>(
                string.Format(ApiConstants.DeviceById, deviceId), dto);
        }

        public async Task<bool> DeleteDeviceAsync(int deviceId)
        {
            return await DeleteAsync(string.Format(ApiConstants.DeviceById, deviceId));
        }

        public async Task<List<DeviceResponseDto>> GetOfflineDevicesAsync(int thresholdMinutes = 30)
        {
            var devices = await GetAsync<List<DeviceResponseDto>>(
                $"{ApiConstants.DevicesOffline}?thresholdMinutes={thresholdMinutes}");
            return devices ?? new List<DeviceResponseDto>();
        }
    }
}
