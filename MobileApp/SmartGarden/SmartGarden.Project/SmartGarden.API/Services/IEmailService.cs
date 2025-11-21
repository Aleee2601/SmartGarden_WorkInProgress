using SmartGarden.Core.Models;

namespace SmartGarden.API.Services;

/// <summary>
/// Email notification service interface
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email notification
    /// </summary>
    Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);

    /// <summary>
    /// Send low water level alert
    /// </summary>
    Task SendLowWaterAlertAsync(User user, Plant plant, int currentLevel);

    /// <summary>
    /// Send device offline alert
    /// </summary>
    Task SendDeviceOfflineAlertAsync(User user, Device device);

    /// <summary>
    /// Send extreme temperature alert
    /// </summary>
    Task SendExtremeTemperatureAlertAsync(User user, Plant plant, double temperature, bool isTooHot);

    /// <summary>
    /// Send low soil moisture alert
    /// </summary>
    Task SendLowSoilMoistureAlertAsync(User user, Plant plant, double moisture);

    /// <summary>
    /// Send daily summary email
    /// </summary>
    Task SendDailySummaryAsync(User user, List<Plant> plants);

    /// <summary>
    /// Send welcome email
    /// </summary>
    Task SendWelcomeEmailAsync(User user);

    /// <summary>
    /// Send device approval notification
    /// </summary>
    Task SendDeviceApprovalNotificationAsync(User user, Device device);
}
