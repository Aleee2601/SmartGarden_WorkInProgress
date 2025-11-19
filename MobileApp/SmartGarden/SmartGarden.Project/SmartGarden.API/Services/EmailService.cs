using System.Net;
using System.Net.Mail;
using SmartGarden.Core.Models;
using Microsoft.Extensions.Configuration;

namespace SmartGarden.API.Services;

/// <summary>
/// Email notification service using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Load SMTP settings from configuration
        _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
        _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@smartgarden.com";
        _fromName = _configuration["EmailSettings:FromName"] ?? "SmartGarden";
        _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
    }

    /// <summary>
    /// Send email using SMTP
    /// </summary>
    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            // Check if SMTP is configured
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogWarning("SMTP credentials not configured. Email not sent.");
                return false;
            }

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = _enableSsl,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                Timeout = 10000 // 10 seconds
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}");
            return false;
        }
    }

    /// <summary>
    /// Send low water level alert
    /// </summary>
    public async Task SendLowWaterAlertAsync(User user, Plant plant, int currentLevel)
    {
        var subject = $"⚠️ Low Water Alert - {plant.Name}";
        var htmlBody = GetLowWaterAlertTemplate(user, plant, currentLevel);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    /// <summary>
    /// Send device offline alert
    /// </summary>
    public async Task SendDeviceOfflineAlertAsync(User user, Device device)
    {
        var subject = $"🔴 Device Offline - {device.DeviceName}";
        var htmlBody = GetDeviceOfflineTemplate(user, device);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    /// <summary>
    /// Send extreme temperature alert
    /// </summary>
    public async Task SendExtremeTemperatureAlertAsync(User user, Plant plant, double temperature, bool isTooHot)
    {
        var subject = isTooHot
            ? $"🔥 High Temperature Alert - {plant.Name}"
            : $"❄️ Low Temperature Alert - {plant.Name}";
        var htmlBody = GetExtremeTemperatureTemplate(user, plant, temperature, isTooHot);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    /// <summary>
    /// Send low soil moisture alert
    /// </summary>
    public async Task SendLowSoilMoistureAlertAsync(User user, Plant plant, double moisture)
    {
        var subject = $"🌱 Low Soil Moisture - {plant.Name}";
        var htmlBody = GetLowSoilMoistureTemplate(user, plant, moisture);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    /// <summary>
    /// Send daily summary email
    /// </summary>
    public async Task SendDailySummaryAsync(User user, List<Plant> plants)
    {
        var subject = $"📊 Daily Summary - {DateTime.Now:MMMM dd, yyyy}";
        var htmlBody = GetDailySummaryTemplate(user, plants);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    /// <summary>
    /// Send welcome email
    /// </summary>
    public async Task SendWelcomeEmailAsync(User user)
    {
        var subject = "🌱 Welcome to Bloomly SmartGarden!";
        var htmlBody = GetWelcomeEmailTemplate(user);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    /// <summary>
    /// Send device approval notification
    /// </summary>
    public async Task SendDeviceApprovalNotificationAsync(User user, Device device)
    {
        var subject = "🔔 New Device Pending Approval";
        var htmlBody = GetDeviceApprovalTemplate(user, device);
        await SendEmailAsync(user.Email, subject, htmlBody);
    }

    #region Email Templates

    private string GetEmailHeader()
    {
        return @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: linear-gradient(135deg, #10b981 0%, #059669 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }
        .content { background: #f9fafb; padding: 30px; border-radius: 0 0 10px 10px; }
        .alert-box { background: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 20px 0; border-radius: 5px; }
        .info-box { background: #eff6ff; border-left: 4px solid #3b82f6; padding: 15px; margin: 20px 0; border-radius: 5px; }
        .button { display: inline-block; background: #10b981; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 10px 0; }
        .footer { text-align: center; color: #6b7280; font-size: 12px; margin-top: 30px; }
        .stats { display: flex; justify-content: space-around; margin: 20px 0; }
        .stat { text-align: center; }
        .stat-value { font-size: 24px; font-weight: bold; color: #10b981; }
        .stat-label { font-size: 12px; color: #6b7280; }
    </style>
</head>
<body>
    <div class='container'>";
    }

    private string GetEmailFooter()
    {
        return @"
        <div class='footer'>
            <p>You're receiving this email because you're using Bloomly SmartGarden.</p>
            <p>© 2024 SmartGarden. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GetLowWaterAlertTemplate(User user, Plant plant, int currentLevel)
    {
        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>⚠️ Low Water Alert</h1>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <div class='alert-box'>
                <h3>Your plant needs attention!</h3>
                <p><strong>{plant.Name}</strong> ({plant.Species}) has a low water level.</p>
                <p><strong>Current Water Level:</strong> {currentLevel}%</p>
            </div>
            <p>Please refill the water tank to ensure your plant stays healthy.</p>
            <a href='#' class='button'>View Plant Details</a>
        </div>" + GetEmailFooter();
    }

    private string GetDeviceOfflineTemplate(User user, Device device)
    {
        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>🔴 Device Offline</h1>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <div class='alert-box'>
                <h3>Device Connection Lost</h3>
                <p><strong>{device.DeviceName}</strong> is currently offline.</p>
                <p>Last seen: {device.LastHeartbeat?.ToString("g") ?? "Unknown"}</p>
            </div>
            <p><strong>Troubleshooting steps:</strong></p>
            <ul>
                <li>Check if the device has power</li>
                <li>Verify WiFi connection</li>
                <li>Restart the device if necessary</li>
            </ul>
        </div>" + GetEmailFooter();
    }

    private string GetExtremeTemperatureTemplate(User user, Plant plant, double temperature, bool isTooHot)
    {
        var emoji = isTooHot ? "🔥" : "❄️";
        var condition = isTooHot ? "high" : "low";
        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>{emoji} Temperature Alert</h1>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <div class='alert-box'>
                <h3>Extreme Temperature Detected</h3>
                <p><strong>{plant.Name}</strong> is experiencing {condition} temperature.</p>
                <p><strong>Current Temperature:</strong> {temperature:F1}°C</p>
                <p><strong>Ideal Range:</strong> {plant.MinTemperature}°C - {plant.MaxTemperature}°C</p>
            </div>
            <p>Consider moving your plant to a more suitable location.</p>
        </div>" + GetEmailFooter();
    }

    private string GetLowSoilMoistureTemplate(User user, Plant plant, double moisture)
    {
        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>🌱 Soil Moisture Alert</h1>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <div class='alert-box'>
                <h3>Your plant is thirsty!</h3>
                <p><strong>{plant.Name}</strong> has low soil moisture.</p>
                <p><strong>Current Moisture:</strong> {moisture:F1}%</p>
                <p><strong>Minimum Required:</strong> {plant.MinSoilMoisture}%</p>
            </div>
            <p>Your plant will be watered automatically if auto-watering is enabled. Otherwise, please water manually.</p>
            <a href='#' class='button'>Water Now</a>
        </div>" + GetEmailFooter();
    }

    private string GetDailySummaryTemplate(User user, List<Plant> plants)
    {
        var plantStats = string.Join("", plants.Select(p => $@"
            <div style='background: white; padding: 15px; margin: 10px 0; border-radius: 8px; border-left: 4px solid #10b981;'>
                <h4 style='margin: 0 0 10px 0;'>{p.Name}</h4>
                <p style='margin: 5px 0; color: #6b7280;'>{p.Species}</p>
                <p style='margin: 5px 0;'>Status: <strong style='color: #10b981;'>Healthy</strong></p>
            </div>
        "));

        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>📊 Daily Summary</h1>
            <p>{DateTime.Now:MMMM dd, yyyy}</p>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <p>Here's your daily plant care summary:</p>
            <div class='stats'>
                <div class='stat'>
                    <div class='stat-value'>{plants.Count}</div>
                    <div class='stat-label'>Plants</div>
                </div>
                <div class='stat'>
                    <div class='stat-value'>{plants.Count(p => p.AutoWateringEnabled)}</div>
                    <div class='stat-label'>Auto-Watering</div>
                </div>
            </div>
            <h3>Your Plants:</h3>
            {plantStats}
            <a href='#' class='button'>View Dashboard</a>
        </div>" + GetEmailFooter();
    }

    private string GetWelcomeEmailTemplate(User user)
    {
        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>🌱 Welcome to Bloomly!</h1>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <p>Thank you for joining <strong>Bloomly SmartGarden</strong>! We're excited to help you take care of your plants.</p>
            <div class='info-box'>
                <h3>Getting Started:</h3>
                <ol>
                    <li>Set up your ESP32 device</li>
                    <li>Approve the device in your dashboard</li>
                    <li>Add your first plant</li>
                    <li>Calibrate your sensors</li>
                    <li>Enable auto-watering</li>
                </ol>
            </div>
            <p>You'll receive email notifications when your plants need attention.</p>
            <a href='#' class='button'>Go to Dashboard</a>
        </div>" + GetEmailFooter();
    }

    private string GetDeviceApprovalTemplate(User user, Device device)
    {
        return GetEmailHeader() + $@"
        <div class='header'>
            <h1>🔔 New Device Detected</h1>
        </div>
        <div class='content'>
            <p>Hi {user.Username},</p>
            <div class='info-box'>
                <h3>Device Pending Approval</h3>
                <p>A new device is requesting access to your SmartGarden account.</p>
                <p><strong>Device Name:</strong> {device.DeviceName}</p>
                <p><strong>MAC Address:</strong> {device.MacAddress}</p>
                <p><strong>Model:</strong> {device.Model ?? "Unknown"}</p>
            </div>
            <p>Please approve this device in your dashboard if you recognize it.</p>
            <a href='#' class='button'>Approve Device</a>
        </div>" + GetEmailFooter();
    }

    #endregion
}
