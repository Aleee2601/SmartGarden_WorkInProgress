# SmartGarden Database Guide 🗄️

## Overview

The SmartGarden system uses **SQL Server** as its primary database, managed through **Entity Framework Core 10**. This guide covers database setup, schema, migrations, optimization, and maintenance.

## Database Schema

### Entity Relationship Diagram

```
┌─────────────┐       ┌──────────────┐       ┌─────────────────┐
│    User     │───────│    Plant     │───────│ SensorReading   │
│             │ 1   * │              │ 1   * │                 │
│ UserId (PK) │       │ PlantId (PK) │       │ ReadingId (PK)  │
│ Username    │       │ UserId (FK)  │       │ PlantId (FK)    │
│ Email       │       │ DeviceId(FK) │       │ SoilMoisture    │
│ Password    │       │ Nickname     │       │ WaterLevel      │
└─────────────┘       │ Species      │       │ AirTemp         │
                      │ Threshold    │       │ Timestamp       │
                      └──────────────┘       └─────────────────┘
                            │ 1
                            │
                            │ *
                      ┌──────────────┐
                      │ WateringLog  │
                      │              │
                      │ LogId (PK)   │
                      │ PlantId (FK) │
                      │ Duration     │
                      │ WateredAt    │
                      └──────────────┘

┌─────────────┐       ┌──────────────┐       ┌─────────────────┐
│   Device    │───────│    Plant     │───────│     Alert       │
│             │ 1   * │              │ 1   * │                 │
│ DeviceId    │       │              │       │ AlertId (PK)    │
│ MacAddress  │       │              │       │ PlantId (FK)    │
│ IsApproved  │       │              │       │ AlertType       │
└─────────────┘       └──────────────┘       │ Severity        │
                                              └─────────────────┘
```

## Database Tables

### 1. Users
Stores user account information.

```sql
CREATE TABLE Users (
    UserId          INT IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(MAX) NOT NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted       BIT NOT NULL DEFAULT 0,
    DeletedAt       DATETIME2 NULL,

    INDEX IX_Users_Email (Email),
    INDEX IX_Users_CreatedAt (CreatedAt)
);
```

**Key Fields:**
- `UserId`: Primary key (identity)
- `Email`: Unique, used for login
- `PasswordHash`: BCrypt hashed password
- `IsDeleted`: Soft delete flag
- `CreatedAt`, `UpdatedAt`: Audit timestamps

### 2. Devices
ESP32 and IoT device registry.

```sql
CREATE TABLE Devices (
    DeviceId        INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    MacAddress      NVARCHAR(17) NOT NULL UNIQUE,
    DeviceName      NVARCHAR(100) NOT NULL,
    FirmwareVersion NVARCHAR(20) NULL,
    IsApproved      BIT NOT NULL DEFAULT 0,
    LastSeen        DATETIME2 NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted       BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Devices_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,

    INDEX IX_Devices_MacAddress (MacAddress),
    INDEX IX_Devices_UserId (UserId),
    INDEX IX_Devices_IsApproved (IsApproved)
);
```

**Key Fields:**
- `MacAddress`: Unique device identifier (format: "AA:BB:CC:DD:EE:FF")
- `IsApproved`: User must approve device before it can send data
- `LastSeen`: Last telemetry timestamp
- `FirmwareVersion`: OTA update tracking

### 3. Plants
Plant configuration and metadata.

```sql
CREATE TABLE Plants (
    PlantId         INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    DeviceId        INT NULL,
    Nickname        NVARCHAR(200) NULL,
    Species         NVARCHAR(200) NULL,
    ScientificName  NVARCHAR(200) NULL,
    SuggestedMoistureThreshold INT NULL,
    WateringFrequency NVARCHAR(50) NULL,
    Sunlight        NVARCHAR(50) NULL,
    CareNotes       NVARCHAR(MAX) NULL,
    LastWateredDate DATETIME2 NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted       BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Plants_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Plants_Devices FOREIGN KEY (DeviceId)
        REFERENCES Devices(DeviceId) ON DELETE SET NULL,

    INDEX IX_Plants_UserId (UserId),
    INDEX IX_Plants_DeviceId (DeviceId),
    INDEX IX_Plants_CreatedAt (CreatedAt)
);
```

**Key Fields:**
- `Nickname`: User-friendly name (e.g., "My Monstera")
- `Species`: Common name (e.g., "Monstera Deliciosa")
- `ScientificName`: Botanical name
- `SuggestedMoistureThreshold`: 0-100% (from Perenual API)
- `LastWateredDate`: Timestamp of last watering (updated via `ExecuteUpdateAsync`)

### 4. SensorReadings
Time-series sensor data.

```sql
CREATE TABLE SensorReadings (
    ReadingId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId         INT NOT NULL,
    SoilMoisture    FLOAT NOT NULL,
    WaterLevel      FLOAT NOT NULL,
    AirTemp         FLOAT NULL,
    AirHumidity     FLOAT NULL,
    LightLevel      FLOAT NULL,
    AirQuality      FLOAT NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_SensorReadings_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,

    INDEX IX_SensorReadings_PlantId_CreatedAt (PlantId, CreatedAt DESC),
    INDEX IX_SensorReadings_CreatedAt (CreatedAt DESC)
);
```

**Key Fields:**
- `SoilMoisture`: 0-100% (capacitive sensor)
- `WaterLevel`: 0-100% (tank level)
- `AirTemp`: °Celsius
- `AirHumidity`: 0-100% RH
- `LightLevel`: Lux (0-65535)
- `AirQuality`: 0-100% (composite score)

**Indexing Strategy:**
- Composite index on `(PlantId, CreatedAt DESC)` for efficient time-series queries
- Separate index on `CreatedAt` for analytics aggregation

### 5. WateringLogs
Historical watering events.

```sql
CREATE TABLE WateringLogs (
    LogId           BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId         INT NOT NULL,
    WateredAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Duration        INT NOT NULL, -- milliseconds
    IsAutomatic     BIT NOT NULL DEFAULT 1,
    SoilMoistureBefore FLOAT NULL,
    WaterLevelBefore FLOAT NULL,

    CONSTRAINT FK_WateringLogs_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,

    INDEX IX_WateringLogs_PlantId_WateredAt (PlantId, WateredAt DESC),
    INDEX IX_WateringLogs_WateredAt (WateredAt DESC)
);
```

**Key Fields:**
- `Duration`: Watering duration in milliseconds (typically 1000-5000ms)
- `IsAutomatic`: TRUE = ESP32 auto-watering, FALSE = manual
- `SoilMoistureBefore`: Moisture reading that triggered watering

### 6. Alerts
System-generated alerts and notifications.

```sql
CREATE TABLE Alerts (
    AlertId         BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,
    PlantId         INT NULL,
    AlertType       NVARCHAR(50) NOT NULL,
    Severity        NVARCHAR(20) NOT NULL,
    Title           NVARCHAR(200) NOT NULL,
    Message         NVARCHAR(MAX) NOT NULL,
    IsRead          BIT NOT NULL DEFAULT 0,
    IsResolved      BIT NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ResolvedAt      DATETIME2 NULL,

    CONSTRAINT FK_Alerts_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Alerts_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE SET NULL,

    INDEX IX_Alerts_UserId_CreatedAt (UserId, CreatedAt DESC),
    INDEX IX_Alerts_IsRead_IsResolved (IsRead, IsResolved)
);
```

**Alert Types:**
- `LowSoilMoisture`: Soil too dry
- `LowWaterLevel`: Tank needs refill
- `HighTemperature`: Air temp too high
- `DeviceOffline`: Device hasn't reported in 1 hour
- `PumpFailure`: Pump didn't activate

**Severity Levels:**
- `Info`: Informational
- `Warning`: Attention needed
- `Critical`: Immediate action required

## Entity Framework Core Configuration

### DbContext

```csharp
public class SmartGardenContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Plant> Plants { get; set; }
    public DbSet<SensorReading> SensorReadings { get; set; }
    public DbSet<WateringLog> WateringLogs { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Soft Delete Global Query Filter
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        modelBuilder.Entity<Plant>()
            .HasQueryFilter(p => !p.IsDeleted);

        // Relationships
        modelBuilder.Entity<Plant>()
            .HasOne(p => p.User)
            .WithMany(u => u.Plants)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        modelBuilder.Entity<SensorReading>()
            .HasIndex(sr => new { sr.PlantId, sr.CreatedAt });

        // Shadow Properties for Auditing
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("CreatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("UpdatedAt")
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit timestamps
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
```

## Database Setup

### Prerequisites

**SQL Server Editions:**
- **LocalDB** (Development) - Included with Visual Studio
- **SQL Server Express** (Free) - Small deployments
- **SQL Server Standard/Enterprise** (Production) - Large scale
- **Azure SQL Database** (Cloud) - Managed service

### Installation

#### Windows - SQL Server LocalDB

```bash
# Check if installed
sqllocaldb info

# Create instance
sqllocaldb create mssqllocaldb

# Start instance
sqllocaldb start mssqllocaldb

# Get connection string
sqllocaldb info mssqllocaldb
```

#### Windows - SQL Server Express

1. Download SQL Server Express from Microsoft
2. Run installer
3. Choose "Basic" installation
4. Note the connection string (e.g., `localhost\SQLEXPRESS`)

#### Docker - SQL Server

```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 \
  --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Connection Strings

#### LocalDB (Development)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartGardenDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### SQL Server Express (Windows Authentication)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SmartGardenDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

#### SQL Server (SQL Authentication)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartGardenDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=true"
  }
}
```

#### Azure SQL Database
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=SmartGardenDb;User ID=yourusername;Password=yourpassword;Encrypt=true;Connection Timeout=30;"
  }
}
```

## Migrations

### Entity Framework Core Migrations

#### Create Initial Migration

```bash
cd MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.Data

dotnet ef migrations add InitialCreate \
  --startup-project ../SmartGarden.API \
  --context SmartGardenContext
```

#### Apply Migration to Database

```bash
dotnet ef database update \
  --startup-project ../SmartGarden.API \
  --context SmartGardenContext
```

#### Add New Migration

```bash
# After modifying entities
dotnet ef migrations add AddLastWateredDateToPlant \
  --startup-project ../SmartGarden.API
```

#### Rollback Migration

```bash
# Rollback to specific migration
dotnet ef database update PreviousMigrationName \
  --startup-project ../SmartGarden.API

# Rollback all migrations
dotnet ef database update 0 \
  --startup-project ../SmartGarden.API
```

#### Remove Last Migration (Before Applying)

```bash
dotnet ef migrations remove \
  --startup-project ../SmartGarden.API
```

#### Generate SQL Script (For Production)

```bash
# Generate script for specific migration range
dotnet ef migrations script InitialCreate AddLastWateredDateToPlant \
  --startup-project ../SmartGarden.API \
  --output migration.sql \
  --idempotent
```

### Migration Best Practices

1. **Always review generated migrations** before applying
2. **Test migrations** on development database first
3. **Use idempotent scripts** for production deployments
4. **Backup database** before applying migrations
5. **Version control migrations** in Git
6. **Document breaking changes** in migration comments

## Seeding Data

### Development Seed Data

```csharp
public static class DbInitializer
{
    public static async Task SeedAsync(SmartGardenContext context)
    {
        // Check if already seeded
        if (await context.Users.AnyAsync())
            return;

        // Create test user
        var user = new User
        {
            Username = "testuser",
            Email = "test@smartgarden.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Create test device
        var device = new Device
        {
            UserId = user.UserId,
            MacAddress = "AA:BB:CC:DD:EE:FF",
            DeviceName = "Test ESP32",
            FirmwareVersion = "2.0.0",
            IsApproved = true
        };
        context.Devices.Add(device);
        await context.SaveChangesAsync();

        // Create test plant
        var plant = new Plant
        {
            UserId = user.UserId,
            DeviceId = device.DeviceId,
            Nickname = "My Monstera",
            Species = "Monstera Deliciosa",
            SuggestedMoistureThreshold = 40,
            Sunlight = "Bright Indirect",
            WateringFrequency = "Weekly"
        };
        context.Plants.Add(plant);
        await context.SaveChangesAsync();

        // Create sample sensor readings
        var readings = new List<SensorReading>();
        for (int i = 0; i < 100; i++)
        {
            readings.Add(new SensorReading
            {
                PlantId = plant.PlantId,
                SoilMoisture = 40 + (i % 30),
                WaterLevel = 80 - (i % 20),
                AirTemp = 22 + (i % 5),
                AirHumidity = 60 + (i % 15),
                LightLevel = 500 + (i % 300),
                CreatedAt = DateTime.UtcNow.AddHours(-i)
            });
        }
        context.SensorReadings.AddRange(readings);
        await context.SaveChangesAsync();
    }
}
```

Call in `Program.cs`:
```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SmartGardenContext>();
    await DbInitializer.SeedAsync(context);
}
```

## Query Optimization

### EF Core 10 ExecuteUpdateAsync

**Traditional Approach** (loads entity, tracks changes):
```csharp
var plant = await _context.Plants.FindAsync(plantId);
plant.LastWateredDate = DateTime.UtcNow;
await _context.SaveChangesAsync();
```

**Optimized Approach** (direct SQL UPDATE):
```csharp
await _context.Plants
    .Where(p => p.PlantId == plantId)
    .ExecuteUpdateAsync(setter => setter
        .SetProperty(p => p.LastWateredDate, DateTime.UtcNow));
```

**Performance Gain:** 50-70% faster, no change tracking overhead.

### Index Usage

```csharp
// Good: Uses index IX_SensorReadings_PlantId_CreatedAt
var readings = await _context.SensorReadings
    .Where(sr => sr.PlantId == plantId && sr.CreatedAt >= startDate)
    .OrderByDescending(sr => sr.CreatedAt)
    .Take(100)
    .ToListAsync();

// Bad: Full table scan
var readings = await _context.SensorReadings
    .Where(sr => sr.CreatedAt.Date == DateTime.Today) // Function on column
    .ToListAsync();
```

### Projection to DTOs

```csharp
// Good: Only select needed fields
var plants = await _context.Plants
    .Where(p => p.UserId == userId)
    .Select(p => new PlantDto
    {
        PlantId = p.PlantId,
        Nickname = p.Nickname,
        Species = p.Species
    })
    .ToListAsync();

// Bad: Loads entire entity with all relationships
var plants = await _context.Plants
    .Include(p => p.SensorReadings)
    .Include(p => p.WateringLogs)
    .Where(p => p.UserId == userId)
    .ToListAsync();
```

### Batch Operations

```csharp
// Good: Single database roundtrip
var readings = new List<SensorReading> { /* ... */ };
_context.SensorReadings.AddRange(readings);
await _context.SaveChangesAsync();

// Bad: Multiple roundtrips
foreach (var reading in readings)
{
    _context.SensorReadings.Add(reading);
    await _context.SaveChangesAsync(); // ❌ Don't do this
}
```

## Database Maintenance

### Regular Maintenance Tasks

#### 1. Index Maintenance (Weekly)

```sql
-- Rebuild fragmented indexes
DECLARE @TableName NVARCHAR(255)
DECLARE @IndexName NVARCHAR(255)
DECLARE TableCursor CURSOR FOR
SELECT OBJECT_NAME(i.object_id), i.name
FROM sys.indexes i
JOIN sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ps
    ON i.object_id = ps.object_id AND i.index_id = ps.index_id
WHERE ps.avg_fragmentation_in_percent > 30

OPEN TableCursor
FETCH NEXT FROM TableCursor INTO @TableName, @IndexName

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC('ALTER INDEX ' + @IndexName + ' ON ' + @TableName + ' REBUILD')
    FETCH NEXT FROM TableCursor INTO @TableName, @IndexName
END

CLOSE TableCursor
DEALLOCATE TableCursor
```

#### 2. Statistics Update (Daily)

```sql
-- Update statistics for better query plans
EXEC sp_updatestats;
```

#### 3. Old Data Archival (Monthly)

```sql
-- Archive sensor readings older than 6 months
INSERT INTO SensorReadingsArchive
SELECT * FROM SensorReadings
WHERE CreatedAt < DATEADD(MONTH, -6, GETUTCDATE());

DELETE FROM SensorReadings
WHERE CreatedAt < DATEADD(MONTH, -6, GETUTCDATE());
```

#### 4. Database Backup (Daily)

```sql
-- Full backup
BACKUP DATABASE SmartGardenDb
TO DISK = 'C:\Backups\SmartGardenDb_Full.bak'
WITH FORMAT, INIT, COMPRESSION;

-- Transaction log backup (every hour)
BACKUP LOG SmartGardenDb
TO DISK = 'C:\Backups\SmartGardenDb_Log.trn'
WITH COMPRESSION;
```

### Performance Monitoring

#### Query Performance

```sql
-- Find slow queries
SELECT TOP 10
    qs.execution_count,
    qs.total_elapsed_time / 1000000.0 AS total_elapsed_time_seconds,
    qs.total_elapsed_time / qs.execution_count / 1000000.0 AS avg_elapsed_time_seconds,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2)+1) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qt.dbid = DB_ID('SmartGardenDb')
ORDER BY qs.total_elapsed_time DESC;
```

#### Index Usage

```sql
-- Find unused indexes
SELECT
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s
    ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE s.index_id IS NULL
    AND i.type_desc <> 'HEAP'
    AND OBJECT_SCHEMA_NAME(i.object_id) = 'dbo';
```

#### Database Size

```sql
-- Check table sizes
SELECT
    t.NAME AS TableName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 / 1024 AS TotalSpaceMB,
    SUM(a.used_pages) * 8 / 1024 AS UsedSpaceMB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 / 1024 AS UnusedSpaceMB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.is_ms_shipped = 0
GROUP BY t.Name, p.Rows
ORDER BY TotalSpaceMB DESC;
```

## Troubleshooting

### Connection Issues

**Problem:** Cannot connect to database

**Solutions:**
```bash
# Check SQL Server is running
# For LocalDB:
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb

# For SQL Server service:
# Windows: Services → SQL Server (MSSQLSERVER) → Start

# Test connection with sqlcmd
sqlcmd -S "(localdb)\mssqllocaldb" -E -Q "SELECT @@VERSION"
```

### Migration Errors

**Problem:** Migration fails with constraint error

**Solution:**
```bash
# Drop and recreate database (DEVELOPMENT ONLY)
dotnet ef database drop --force --startup-project ../SmartGarden.API
dotnet ef database update --startup-project ../SmartGarden.API
```

### Performance Issues

**Problem:** Slow query performance

**Solutions:**
1. Add appropriate indexes
2. Use projection to DTOs
3. Implement query result caching
4. Use `ExecuteUpdateAsync` for bulk updates
5. Check query execution plans
6. Update statistics

## Related Documentation

- [Main Project Documentation](./PROJECT_DOCUMENTATION.md)
- [Backend API README](./MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/README.md)
- [API Reference](./API_REFERENCE.md)

---

**Version:** 1.0
**Last Updated:** November 2025
**Database:** SQL Server 2022 / Azure SQL
**ORM:** Entity Framework Core 10
