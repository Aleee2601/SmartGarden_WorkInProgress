namespace SmartGarden_WorkInProgress.Models;

public class Metric
{
    public string Label { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
}
