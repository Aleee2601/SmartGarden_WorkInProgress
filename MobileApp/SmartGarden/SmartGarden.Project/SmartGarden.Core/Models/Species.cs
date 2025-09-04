namespace SmartGarden.Core.Models
{
    public class Species
    {
        public int SpeciesId { get; set; }                       // necesar (eroarea ta)
        public string CommonName { get; set; } = null!;
        public string ScientificName { get; set; } = null!;
        public double DefaultSoilMoistMin { get; set; }
        public double DefaultSoilMoistMax { get; set; }
        public double DefaultTempMin { get; set; }
        public double DefaultTempMax { get; set; }
        public double DefaultLightMin { get; set; }
        public double DefaultLightMax { get; set; }
        public double DefaultHumidityMin { get; set; }
        public double DefaultHumidityMax { get; set; }
    }
}
