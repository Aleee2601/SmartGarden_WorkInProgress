using SmartGarden.Core.DTOs;
using SmartGarden.Core.Shared;

namespace SmartGarden.App.Services
{
    public interface IAlertService
    {
        Task<List<AlertResponseDto>> GetUserAlertsAsync(int userId);
        Task<List<AlertResponseDto>> GetUnreadAlertsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<AlertResponseDto?> GetAlertByIdAsync(int alertId);
        Task<AlertResponseDto?> CreateAlertAsync(CreateAlertDto dto);
        Task<bool> MarkAsReadAsync(int alertId);
        Task<bool> DismissAlertAsync(int alertId);
        Task<bool> ResolveAlertAsync(int alertId);
        Task<List<AlertResponseDto>> GetBySeverityAsync(int userId, AlertSeverity severity);
    }
}
