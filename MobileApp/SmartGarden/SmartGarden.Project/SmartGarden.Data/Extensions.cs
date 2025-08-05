using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SmartGarden.Data
{
    public static class Extensions
    {
        public static IServiceCollection AddSmartGardenData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SmartGardenDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
