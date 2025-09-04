namespace SmartGarden.Core.Models
{
    public class SoilType
    {
        public int SoilTypeId { get; set; }                      // necesar (eroarea ta)
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int RecWaterDueSec { get; set; } = 5;             // necesar (eroarea ta)
        public int PauseBetweenWaterMin { get; set; } = 2;
    }
}
