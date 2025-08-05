using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.Core.Interfaces
{
    public interface ISensorService
    {
        Task<IEnumerable<SensorReading>> GetReadingsForPlantAsync(int plantId);
        Task<SensorReading> AddReadingAsync(int plantId, CreateSensorReadingDto dto);
    }
}
