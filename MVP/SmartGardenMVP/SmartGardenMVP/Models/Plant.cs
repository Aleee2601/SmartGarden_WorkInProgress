using System;
using System.Collections.Generic;

namespace SmartGarden.API.Models
{
    public class Plant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int MoistureMin { get; set; }
        public int MoistureMax { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<IrrigationEvent> IrrigationEvents { get; set; }
        public bool AutoIrrigationEnabled { get; set; } = false;
        public Guid UserId { get; set; }
        public User User { get; set; }


    }
}
