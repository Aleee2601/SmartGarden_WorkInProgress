using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence;

public class SmartGardenDbContext : DbContext
{
    public SmartGardenDbContext(DbContextOptions<SmartGardenDbContext> options) : base(options) { }

    // DbSet-uri
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Species> Species => Set<Species>();
    public DbSet<SoilType> SoilTypes => Set<SoilType>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<WateringLog> WateringLogs => Set<WateringLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Încarcă toate mapările Fluent API din assembly-ul curent
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartGardenDbContext).Assembly);

        // Audit & soft delete (shadow properties) + query filters per entitate
        ConfigureAuditAndSoftDelete<User>(modelBuilder);
        ConfigureAuditAndSoftDelete<UserSetting>(modelBuilder);
        ConfigureAuditAndSoftDelete<Plant>(modelBuilder);
        ConfigureAuditAndSoftDelete<Species>(modelBuilder);
        ConfigureAuditAndSoftDelete<SoilType>(modelBuilder);
        ConfigureAuditAndSoftDelete<SensorReading>(modelBuilder);
        ConfigureAuditAndSoftDelete<WateringLog>(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureAuditAndSoftDelete<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        var e = modelBuilder.Entity<TEntity>();

        // Shadow props (nu apar în clasele din Core)
        e.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("CURRENT_TIMESTAMP"); // valid și pe SQL Server
        e.Property<DateTime?>("UpdatedAt");
        e.Property<bool>("IsDeleted")
            .HasDefaultValue(false);

        // Global query filter pe IsDeleted == false
        e.HasQueryFilter(BuildIsDeletedFilter<TEntity>());
    }

    private static Expression<Func<TEntity, bool>> BuildIsDeletedFilter<TEntity>()
        where TEntity : class
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var prop = Expression.Call(
            typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(bool)),
            param,
            Expression.Constant("IsDeleted"));
        var body = Expression.Equal(prop, Expression.Constant(false));
        return Expression.Lambda<Func<TEntity, bool>>(body, param);
    }

    /// <summary>
    /// Interceptează SaveChanges pentru a popula CreatedAt/UpdatedAt și pentru soft delete.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries().Where(e =>
                     e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            // Soft delete
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Property("IsDeleted").CurrentValue = true;
                entry.Property("UpdatedAt").CurrentValue = now;
                continue;
            }

            // Added / Modified
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = now;
                entry.Property("IsDeleted").CurrentValue = false;
            }

            entry.Property("UpdatedAt").CurrentValue = now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    // ---------- Helpers cerute în blueprint ----------

    /// <summary>
    /// Batch insert pentru citiri senzori (optimizat pentru volum).
    /// </summary>
    public async Task AddSensorReadingsBatchAsync(IEnumerable<SensorReading> readings,
        CancellationToken ct = default)
    {
        // IMPORTANT: ca să beneficiezi de "NoTracking" global, atasăm direct și setăm shadow props
        var now = DateTime.UtcNow;
        foreach (var r in readings)
        {
            var entry = Entry(r);
            entry.Property("CreatedAt").CurrentValue = now;
            entry.Property("UpdatedAt").CurrentValue = now;
            entry.Property("IsDeleted").CurrentValue = false;
        }

        await SensorReadings.AddRangeAsync(readings, ct);
        await SaveChangesAsync(ct);
    }

    /// <summary>
    /// Șterge fizic citirile mai vechi decât o dată (retenție).
    /// </summary>
    public Task<int> PurgeOldSensorReadingsAsync(DateTime olderThanUtc, CancellationToken ct = default)
    {
        // EF Core 7/8: ExecuteDelete
        return SensorReadings
            .IgnoreQueryFilters()
            .Where(r => EF.Property<DateTime>(r, "CreatedAt") < olderThanUtc)
            .ExecuteDeleteAsync(ct);
    }

    /// <summary>
    /// Scurtătură: reține ultimele N zile.
    /// </summary>
    public Task<int> PurgeOldSensorReadingsAsync(TimeSpan retention, CancellationToken ct = default)
        => PurgeOldSensorReadingsAsync(DateTime.UtcNow.Subtract(retention), ct);
}
