using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartGarden.Data
{
    public class SmartGardenDbContextFactory : IDesignTimeDbContextFactory<SmartGardenDbContext>
    {
        public SmartGardenDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartGardenDbContext>();
            //  Pune aici exact connection string-ul din appsettings.json
            optionsBuilder.UseSqlServer("Server=ALEXANDRA\\SQL2022;Database=SmartGardenDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new SmartGardenDbContext(optionsBuilder.Options);
        }
    }
}
