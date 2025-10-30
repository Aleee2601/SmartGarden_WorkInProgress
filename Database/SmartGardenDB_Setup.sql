-- =============================================
-- SmartGarden Database Setup Script
-- For SQL Server 2022
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'SmartGardenDB')
BEGIN
    ALTER DATABASE SmartGardenDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SmartGardenDB;
END
GO

-- Create database
CREATE DATABASE SmartGardenDB;
GO

USE SmartGardenDB;
GO

-- =============================================
-- Create Tables
-- =============================================

-- Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Name NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE INDEX IX_Users_Email ON Users(Email);
GO

-- UserSettings Table (1-to-1 with Users)
CREATE TABLE UserSettings (
    UserId INT PRIMARY KEY,
    AutoWateringEnabled BIT NOT NULL DEFAULT 0,
    SoilMoistThreshold FLOAT NOT NULL DEFAULT 30.0,
    DataReadIntervalMin INT NOT NULL DEFAULT 15,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

-- Species Table (Lookup/Master Data)
CREATE TABLE Species (
    SpeciesId INT IDENTITY(1,1) PRIMARY KEY,
    CommonName NVARCHAR(100) NOT NULL,
    ScientificName NVARCHAR(MAX) NULL,
    DefaultSoilMoistMin FLOAT NOT NULL DEFAULT 30.0,
    DefaultSoilMoistMax FLOAT NOT NULL DEFAULT 70.0,
    DefaultTempMin FLOAT NOT NULL DEFAULT 15.0,
    DefaultTempMax FLOAT NOT NULL DEFAULT 30.0,
    DefaultLightMin FLOAT NOT NULL DEFAULT 1000.0,
    DefaultLightMax FLOAT NOT NULL DEFAULT 50000.0,
    DefaultHumidityMin FLOAT NOT NULL DEFAULT 40.0,
    DefaultHumidityMax FLOAT NOT NULL DEFAULT 80.0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- SoilTypes Table (Lookup/Master Data)
CREATE TABLE SoilTypes (
    SoilTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    RecWaterDueSec INT NOT NULL DEFAULT 5,
    PauseBetweenWaterMin INT NOT NULL DEFAULT 2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- Plants Table
CREATE TABLE Plants (
    PlantId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    SpeciesId INT NOT NULL,
    SoilTypeId INT NOT NULL,
    Nickname NVARCHAR(80) NULL,
    RoomName NVARCHAR(80) NULL,
    IsOutdoor BIT NOT NULL DEFAULT 0,
    DateAcquired DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Plants_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Plants_Species FOREIGN KEY (SpeciesId)
        REFERENCES Species(SpeciesId) ON DELETE NO ACTION,
    CONSTRAINT FK_Plants_SoilTypes FOREIGN KEY (SoilTypeId)
        REFERENCES SoilTypes(SoilTypeId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Plants_UserId ON Plants(UserId);
CREATE INDEX IX_Plants_SpeciesId ON Plants(SpeciesId);
CREATE INDEX IX_Plants_SoilTypeId ON Plants(SoilTypeId);
CREATE INDEX IX_Plants_UserId_SpeciesId ON Plants(UserId, SpeciesId);
CREATE INDEX IX_Plants_UserId_IsOutdoor ON Plants(UserId, IsOutdoor);
GO

-- SensorReadings Table
CREATE TABLE SensorReadings (
    ReadingId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    SoilMoisture FLOAT NOT NULL,
    AirTemp FLOAT NOT NULL,
    AirHumidity FLOAT NOT NULL,
    LightLevel FLOAT NOT NULL DEFAULT 0,
    AirQuality FLOAT NOT NULL DEFAULT 0,
    WaterLevel FLOAT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_SensorReadings_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_SensorReadings_PlantId ON SensorReadings(PlantId);
CREATE INDEX IX_SensorReadings_CreatedAt ON SensorReadings(CreatedAt DESC);
GO

-- WateringLogs Table
CREATE TABLE WateringLogs (
    WateringId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    DurationSec INT NOT NULL DEFAULT 5,
    Mode INT NOT NULL DEFAULT 0, -- 0 = Manual, 1 = Auto (WateringMode enum)
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_WateringLogs_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_WateringLogs_PlantId ON WateringLogs(PlantId);
CREATE INDEX IX_WateringLogs_CreatedAt ON WateringLogs(CreatedAt DESC);
GO

-- =============================================
-- EF Core Migration History Table
-- =============================================

CREATE TABLE __EFMigrationsHistory (
    MigrationId NVARCHAR(150) PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
);
GO

-- Insert existing migrations (so EF Core knows they're applied)
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES
    ('20250806134551_InitialCreate', '9.0.8'),
    ('20250806143925_AddPlantRelations', '9.0.8');
GO

-- =============================================
-- Seed Data for Lookup Tables
-- =============================================

-- Insert default Species
INSERT INTO Species (CommonName, ScientificName, DefaultSoilMoistMin, DefaultSoilMoistMax, DefaultTempMin, DefaultTempMax, DefaultLightMin, DefaultLightMax, DefaultHumidityMin, DefaultHumidityMax)
VALUES
    ('Basil', 'Ocimum basilicum', 40.0, 70.0, 18.0, 27.0, 6000.0, 15000.0, 50.0, 70.0),
    ('Tomato', 'Solanum lycopersicum', 40.0, 80.0, 20.0, 30.0, 8000.0, 20000.0, 60.0, 80.0),
    ('Lettuce', 'Lactuca sativa', 50.0, 75.0, 15.0, 20.0, 4000.0, 10000.0, 60.0, 80.0),
    ('Mint', 'Mentha', 50.0, 80.0, 15.0, 25.0, 3000.0, 8000.0, 60.0, 85.0),
    ('Cactus', 'Cactaceae', 10.0, 30.0, 20.0, 35.0, 10000.0, 30000.0, 20.0, 40.0),
    ('Fern', 'Polypodiopsida', 60.0, 90.0, 18.0, 24.0, 1000.0, 5000.0, 70.0, 90.0),
    ('Spider Plant', 'Chlorophytum comosum', 40.0, 60.0, 18.0, 27.0, 3000.0, 10000.0, 40.0, 70.0),
    ('Pothos', 'Epipremnum aureum', 40.0, 60.0, 18.0, 29.0, 2000.0, 8000.0, 50.0, 70.0),
    ('Snake Plant', 'Sansevieria trifasciata', 20.0, 40.0, 18.0, 29.0, 2000.0, 10000.0, 30.0, 50.0),
    ('Aloe Vera', 'Aloe barbadensis miller', 15.0, 35.0, 18.0, 30.0, 8000.0, 20000.0, 30.0, 50.0);
GO

-- Insert default SoilTypes
INSERT INTO SoilTypes (Name, Description, RecWaterDueSec, PauseBetweenWaterMin)
VALUES
    ('Potting Mix', 'Standard indoor potting soil with good drainage', 5, 2),
    ('Cactus Mix', 'Sandy, well-draining soil for succulents and cacti', 3, 5),
    ('Peat Moss', 'High water retention, acidic soil for moisture-loving plants', 7, 1),
    ('Clay Soil', 'Heavy soil with poor drainage, needs less frequent watering', 8, 4),
    ('Sandy Soil', 'Light soil with excellent drainage, needs frequent watering', 4, 1),
    ('Loamy Soil', 'Balanced mix of sand, silt, and clay - ideal for most plants', 5, 2),
    ('Orchid Bark', 'Chunky bark mix for orchids and epiphytes', 3, 3),
    ('Seed Starting Mix', 'Fine, sterile mix for germinating seeds', 4, 2);
GO

-- =============================================
-- Summary
-- =============================================

PRINT '✅ SmartGarden Database Created Successfully!';
PRINT '';
PRINT '📊 Database: SmartGardenDB';
PRINT '📋 Tables Created: 7';
PRINT '   - Users';
PRINT '   - UserSettings';
PRINT '   - Species';
PRINT '   - SoilTypes';
PRINT '   - Plants';
PRINT '   - SensorReadings';
PRINT '   - WateringLogs';
PRINT '';
PRINT '🌱 Seed Data Inserted:';
PRINT '   - 10 Species';
PRINT '   - 8 SoilTypes';
PRINT '';
PRINT '✨ Ready to use with SmartGarden API!';
GO
