using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartGarden.Data.Persistence;

namespace SmartGarden.Data.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Înrolează Data Layer-ul și SmartGardenDbContext în DI.
    /// </summary>
    public static IServiceCollection AddDataLayer(
        this IServiceCollection services,
        IConfiguration config,
        string connectionName = "Default",
        bool useSqliteDev = true)
    {
        var connString = config.GetConnectionString(connectionName)
                         ?? "Data Source=smartgarden.db";

        services.AddDbContextPool<SmartGardenDbContext>(opt =>
        {
            if (useSqliteDev)
            {
                opt.UseSqlite(connString,
                    x => x.MigrationsAssembly(typeof(SmartGardenDbContext).Assembly.FullName));
            }
            else
            {
                opt.UseSqlServer(connString,
                    x => x.MigrationsAssembly(typeof(SmartGardenDbContext).Assembly.FullName));
            }

            // Recomandat pentru scenarii API (se poate schimba local la nevoie)
            opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            opt.EnableSensitiveDataLogging(false);
        });

        return services;
    }
}
