namespace SmartGarden.API.Models
{
    public class PlantType
    {
        public int Id { get; set; }
        public string Name { get; set; } // ex: "Monstera"
        public int DefaultMoistureMin { get; set; }
        public int DefaultMoistureMax { get; set; }
    }
}
