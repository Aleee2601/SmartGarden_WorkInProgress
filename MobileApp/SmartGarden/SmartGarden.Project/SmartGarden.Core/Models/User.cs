using System;
using System.Collections.Generic;

namespace SmartGarden.Core.Models
{
    public class User
    {
        public int UserId { get; set; }                 // necesar (eroarea ta)
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Email notification preferences
        public bool EmailNotificationsEnabled { get; set; } = true;
        public bool NotifyLowWater { get; set; } = true;
        public bool NotifyDeviceOffline { get; set; } = true;
        public bool NotifyExtremeTemperature { get; set; } = true;
        public bool NotifyLowSoilMoisture { get; set; } = true;
        public bool NotifyDailySummary { get; set; } = false;

        public UserSetting? UserSetting { get; set; }   // 1-1
        public ICollection<Plant> Plants { get; set; } = new List<Plant>(); // 1-N
    }
}
