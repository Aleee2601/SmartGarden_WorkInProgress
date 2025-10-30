namespace SmartGarden.Core.Shared
{
    public enum AlertType
    {
        LowMoisture = 0,
        HighMoisture = 1,
        LowTemperature = 2,
        HighTemperature = 3,
        LowHumidity = 4,
        HighHumidity = 5,
        LowLight = 6,
        HighLight = 7,
        LowWaterLevel = 8,
        DeviceOffline = 9,
        PlantNeedsWater = 10,
        MaintenanceDue = 11,
        PoorAirQuality = 12
    }
}
