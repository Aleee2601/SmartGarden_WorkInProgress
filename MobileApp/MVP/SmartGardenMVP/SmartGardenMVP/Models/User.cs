using System;
using System.Collections.Generic;

namespace SmartGarden.API.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<Plant> Plants { get; set; }
    }
}
