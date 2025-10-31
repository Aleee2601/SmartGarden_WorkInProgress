namespace SmartGarden_WorkInProgress.Models;

public class Plant
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public bool IsIndoor { get; set; }
    public string Room { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public int Fertilizer { get; set; }
    public int WaterTank { get; set; }
    public int Light { get; set; }
    public int Temperature { get; set; }
    public int SoilMoisture { get; set; }
    public TimeSpan NextWatering { get; set; }
    public int Weeks { get; set; }
    public bool IsSelected { get; set; }
}
