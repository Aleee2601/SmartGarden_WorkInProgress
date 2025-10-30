using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartGarden.Core.Models;

namespace SmartGarden.Core.Interfaces
{
    public interface IWateringService
    {
        Task<IEnumerable<WateringLog>> GetLogsAsync(int plantId);
        Task<WateringLog> LogWateringAsync(int plantId, string mode, int durationSec = 5);
    }
}
