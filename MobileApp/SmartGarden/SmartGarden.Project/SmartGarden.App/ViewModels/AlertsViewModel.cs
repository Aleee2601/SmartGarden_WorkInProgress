using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGarden.App.Helpers;
using SmartGarden.App.Services;
using SmartGarden.Core.DTOs;
using System.Collections.ObjectModel;

namespace SmartGarden.App.ViewModels
{
    public partial class AlertsViewModel : BaseViewModel
    {
        private readonly IAlertService _alertService;

        [ObservableProperty]
        private ObservableCollection<AlertResponseDto> alerts = new();

        [ObservableProperty]
        private int unreadCount;

        public AlertsViewModel(IAlertService alertService)
        {
            _alertService = alertService;
            Title = "Alerts";
        }

        public override async Task InitializeAsync()
        {
            await LoadAlertsAsync();
        }

        [RelayCommand]
        private async Task LoadAlertsAsync()
        {
            await ExecuteAsync(async () =>
            {
                var userId = await SecureStorageHelper.GetUserIdAsync();
                if (userId.HasValue)
                {
                    var alertsList = await _alertService.GetUserAlertsAsync(userId.Value);
                    Alerts = new ObservableCollection<AlertResponseDto>(alertsList);

                    UnreadCount = await _alertService.GetUnreadCountAsync(userId.Value);
                }
            });
        }

        [RelayCommand]
        private async Task MarkAsReadAsync(AlertResponseDto alert)
        {
            if (alert == null) return;

            await ExecuteAsync(async () =>
            {
                var success = await _alertService.MarkAsReadAsync(alert.AlertId);
                if (success)
                {
                    alert.IsRead = true;
                    UnreadCount--;
                }
            });
        }

        [RelayCommand]
        private async Task DismissAlertAsync(AlertResponseDto alert)
        {
            if (alert == null) return;

            await ExecuteAsync(async () =>
            {
                var success = await _alertService.DismissAlertAsync(alert.AlertId);
                if (success)
                {
                    Alerts.Remove(alert);
                }
            });
        }

        [RelayCommand]
        private async Task RefreshAlertsAsync()
        {
            await LoadAlertsAsync();
        }
    }
}
