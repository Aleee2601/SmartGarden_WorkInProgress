namespace SmartGarden.API.DTOs
{
    public class PlantDTO
    {
        public string Name { get; set; }
        public required string Type { get; set; }
        public int MoistureMin { get; set; }
        public int MoistureMax { get; set; }
    }
}
