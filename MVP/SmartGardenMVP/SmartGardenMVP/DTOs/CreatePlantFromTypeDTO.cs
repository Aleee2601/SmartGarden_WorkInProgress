using System;

namespace SmartGarden.API.DTOs
{
    public class CreatePlantFromTypeDTO
    {
        public string Name { get; set; }       // numele personal al plantei
        public int PlantTypeId { get; set; }   // tipul ales din listă (Monstera, Cactus etc.)
    }
}
