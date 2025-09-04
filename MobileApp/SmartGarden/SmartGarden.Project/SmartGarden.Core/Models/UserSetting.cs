namespace SmartGarden.Core.Models
{
    public class UserSetting
    {
        // PK = FK către User
        public int UserId { get; set; }
        public bool AutoWateringEnabled { get; set; } = false;
        public double SoilMoistThreshold { get; set; } = 30.0; // %
        public int DataReadIntervalMin { get; set; } = 15;

        public User User { get; set; } = null!;
    }
}
