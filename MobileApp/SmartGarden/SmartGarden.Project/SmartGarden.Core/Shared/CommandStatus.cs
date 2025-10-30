namespace SmartGarden.Core.Shared
{
    public enum CommandStatus
    {
        Pending = 0,
        Sent = 1,
        Acknowledged = 2,
        Completed = 3,
        Failed = 4,
        Timeout = 5
    }
}
