using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGarden.Core.Models
{
    public class UserSetting
    {
        public int UserId { get; set; }
        public bool AutoWateringEnabled { get; set; } = true;
        public double SoilMoistThreshold { get; set; } = 30.0;
        public int DataReadIntervalMin { get; set; } = 15;

        public User User { get; set; } = null!;
    }
}
