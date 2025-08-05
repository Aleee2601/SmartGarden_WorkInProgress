using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Models;

namespace SmartGarden.Core.Interfaces
{
    public interface IPlantService
    {
        Task<IEnumerable<Plant>> GetAllAsync();
        Task<Plant?> GetByIdAsync(int id);
        Task<Plant> CreateAsync(CreatePlantDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
