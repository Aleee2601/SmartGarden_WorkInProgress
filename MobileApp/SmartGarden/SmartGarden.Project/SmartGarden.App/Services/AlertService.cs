using SmartGarden.App.Constants;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Shared;

namespace SmartGarden.App.Services
{
    public class AlertService : BaseApiService, IAlertService
    {
        public AlertService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<AlertResponseDto>> GetUserAlertsAsync(int userId)
        {
            var alerts = await GetAsync<List<AlertResponseDto>>(
                string.Format(ApiConstants.AlertsByUser, userId));
            return alerts ?? new List<AlertResponseDto>();
        }

        public async Task<List<AlertResponseDto>> GetUnreadAlertsAsync(int userId)
        {
            var alerts = await GetAsync<List<AlertResponseDto>>(
                string.Format(ApiConstants.AlertsUnread, userId));
            return alerts ?? new List<AlertResponseDto>();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var result = await GetAsync<Dictionary<string, int>>(
                string.Format(ApiConstants.AlertsCount, userId));
            return result?.ContainsKey("count") == true ? result["count"] : 0;
        }

        public async Task<AlertResponseDto?> GetAlertByIdAsync(int alertId)
        {
            return await GetAsync<AlertResponseDto>(
                string.Format(ApiConstants.AlertById, alertId));
        }

        public async Task<AlertResponseDto?> CreateAlertAsync(CreateAlertDto dto)
        {
            return await PostAsync<CreateAlertDto, AlertResponseDto>(
                ApiConstants.AlertsByUser.Replace("/user/{0}", ""), dto);
        }

        public async Task<bool> MarkAsReadAsync(int alertId)
        {
            return await PutAsync(string.Format(ApiConstants.AlertMarkRead, alertId));
        }

        public async Task<bool> DismissAlertAsync(int alertId)
        {
            return await PutAsync(string.Format(ApiConstants.AlertDismiss, alertId));
        }

        public async Task<bool> ResolveAlertAsync(int alertId)
        {
            return await PutAsync(string.Format(ApiConstants.AlertResolve, alertId));
        }

        public async Task<List<AlertResponseDto>> GetBySeverityAsync(int userId, AlertSeverity severity)
        {
            var alerts = await GetAsync<List<AlertResponseDto>>(
                $"{ApiConstants.ApiVersion}/alert/user/{userId}/severity/{(int)severity}");
            return alerts ?? new List<AlertResponseDto>();
        }
    }
}
