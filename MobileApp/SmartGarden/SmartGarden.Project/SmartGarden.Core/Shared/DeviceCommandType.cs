namespace SmartGarden.Core.Shared
{
    public enum DeviceCommandType
    {
        Water = 0,
        ReadSensors = 1,
        UpdateFirmware = 2,
        Reboot = 3,
        Calibrate = 4,
        SetInterval = 5,
        PingHeartbeat = 6
    }
}
