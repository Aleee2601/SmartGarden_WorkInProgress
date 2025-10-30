using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGarden.App.Helpers;
using SmartGarden.App.Services;
using SmartGarden.Core.DTOs;
using System.Collections.ObjectModel;

namespace SmartGarden.App.ViewModels
{
    public partial class DevicesViewModel : BaseViewModel
    {
        private readonly IDeviceService _deviceService;

        [ObservableProperty]
        private ObservableCollection<DeviceResponseDto> devices = new();

        public DevicesViewModel(IDeviceService deviceService)
        {
            _deviceService = deviceService;
            Title = "Devices";
        }

        public override async Task InitializeAsync()
        {
            await LoadDevicesAsync();
        }

        [RelayCommand]
        private async Task LoadDevicesAsync()
        {
            await ExecuteAsync(async () =>
            {
                var userId = await SecureStorageHelper.GetUserIdAsync();
                if (userId.HasValue)
                {
                    var devicesList = await _deviceService.GetUserDevicesAsync(userId.Value);
                    Devices = new ObservableCollection<DeviceResponseDto>(devicesList);
                }
            });
        }

        [RelayCommand]
        private async Task RefreshDevicesAsync()
        {
            await LoadDevicesAsync();
        }
    }
}
