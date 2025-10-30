namespace SmartGarden.App.Constants
{
    public static class ApiConstants
    {
        // Change this to your API URL
        // For Android Emulator: http://10.0.2.2:5000
        // For iOS Simulator: http://localhost:5000
        // For Physical Device: http://YOUR_IP:5000
        public const string BaseUrl = "http://10.0.2.2:5000";

        public const string ApiVersion = "api";

        // Auth Endpoints
        public const string Register = $"{ApiVersion}/auth/register";
        public const string Login = $"{ApiVersion}/auth/login";

        // Plant Endpoints
        public const string Plants = $"{ApiVersion}/plant";
        public const string PlantById = $"{ApiVersion}/plant/{{0}}";

        // Device Endpoints
        public const string Devices = $"{ApiVersion}/device";
        public const string DevicesByUser = $"{ApiVersion}/device/user/{{0}}";
        public const string DeviceById = $"{ApiVersion}/device/{{0}}";
        public const string DeviceHeartbeat = $"{ApiVersion}/device/{{0}}/heartbeat";
        public const string DevicesOffline = $"{ApiVersion}/device/offline";

        // Alert Endpoints
        public const string AlertsByUser = $"{ApiVersion}/alert/user/{{0}}";
        public const string AlertsUnread = $"{ApiVersion}/alert/user/{{0}}/unread";
        public const string AlertsCount = $"{ApiVersion}/alert/user/{{0}}/count";
        public const string AlertById = $"{ApiVersion}/alert/{{0}}";
        public const string AlertMarkRead = $"{ApiVersion}/alert/{{0}}/read";
        public const string AlertDismiss = $"{ApiVersion}/alert/{{0}}/dismiss";
        public const string AlertResolve = $"{ApiVersion}/alert/{{0}}/resolve";

        // Sensor Endpoints
        public const string SensorReading = $"{ApiVersion}/sensor";
        public const string SensorByPlant = $"{ApiVersion}/sensor/plant/{{0}}";
        public const string SensorLatest = $"{ApiVersion}/sensor/plant/{{0}}/latest";

        // Watering Endpoints
        public const string WaterPlant = $"{ApiVersion}/watering/water/{{0}}";
    }
}
