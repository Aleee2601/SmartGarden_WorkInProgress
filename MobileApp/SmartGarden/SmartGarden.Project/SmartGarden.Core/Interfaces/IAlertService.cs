using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;
using SmartGarden.Core.Shared;

namespace SmartGarden.Core.Interfaces
{
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Alert>> GetUnreadAsync(int userId);
        Task<Alert?> GetByIdAsync(int alertId);
        Task<Alert> CreateAsync(CreateAlertDto dto);
        Task<bool> MarkAsReadAsync(int alertId);
        Task<bool> DismissAsync(int alertId);
        Task<bool> ResolveAsync(int alertId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<IEnumerable<Alert>> GetBySeverityAsync(int userId, AlertSeverity severity);
    }
}
