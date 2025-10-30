-- =============================================
-- SmartGarden Database Setup Script - ENHANCED
-- For SQL Server 2022
-- Version: 2.0 - Production Ready
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

-- =============================================
-- CORE TABLES - User Management
-- =============================================

-- Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Name NVARCHAR(100) NULL,

    -- Profile info
    PhoneNumber NVARCHAR(20) NULL,
    TimeZone NVARCHAR(50) NULL DEFAULT 'UTC',
    Language NVARCHAR(10) NULL DEFAULT 'en',

    -- Account status
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    LastLoginAt DATETIME2 NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
CREATE INDEX IX_Users_Email_Active ON Users(Email, IsActive);
GO

-- UserSettings Table
CREATE TABLE UserSettings (
    UserId INT PRIMARY KEY,

    -- Auto-watering settings
    AutoWateringEnabled BIT NOT NULL DEFAULT 0,
    SoilMoistThreshold FLOAT NOT NULL DEFAULT 30.0,
    DataReadIntervalMin INT NOT NULL DEFAULT 15,

    -- Notification preferences
    EnableEmailNotifications BIT NOT NULL DEFAULT 1,
    EnablePushNotifications BIT NOT NULL DEFAULT 1,
    EnableSMSNotifications BIT NOT NULL DEFAULT 0,
    QuietHoursStart TIME NULL,
    QuietHoursEnd TIME NULL,

    -- Display preferences
    TemperatureUnit NVARCHAR(1) NOT NULL DEFAULT 'C',
    Theme NVARCHAR(10) NOT NULL DEFAULT 'light',

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

-- =============================================
-- MASTER DATA TABLES
-- =============================================

-- Species Table
CREATE TABLE Species (
    SpeciesId INT IDENTITY(1,1) PRIMARY KEY,
    CommonName NVARCHAR(100) NOT NULL,
    ScientificName NVARCHAR(200) NULL,
    Description NVARCHAR(MAX) NULL,

    -- Default environmental ranges
    DefaultSoilMoistMin FLOAT NOT NULL DEFAULT 30.0,
    DefaultSoilMoistMax FLOAT NOT NULL DEFAULT 70.0,
    DefaultTempMin FLOAT NOT NULL DEFAULT 15.0,
    DefaultTempMax FLOAT NOT NULL DEFAULT 30.0,
    DefaultLightMin FLOAT NOT NULL DEFAULT 1000.0,
    DefaultLightMax FLOAT NOT NULL DEFAULT 50000.0,
    DefaultHumidityMin FLOAT NOT NULL DEFAULT 40.0,
    DefaultHumidityMax FLOAT NOT NULL DEFAULT 80.0,

    -- Care recommendations
    DefaultWaterFrequencyDays INT NOT NULL DEFAULT 3,
    DefaultFertilizerFrequencyDays INT NULL,
    GrowthRate NVARCHAR(20) NULL,
    MaxHeightCm INT NULL,

    -- Categorization
    Category NVARCHAR(50) NULL,
    Tags NVARCHAR(200) NULL,
    ImageUrl NVARCHAR(500) NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE INDEX IX_Species_CommonName ON Species(CommonName);
CREATE INDEX IX_Species_Category ON Species(Category);
GO

-- SoilTypes Table
CREATE TABLE SoilTypes (
    SoilTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,

    -- Watering characteristics
    RecWaterDueSec INT NOT NULL DEFAULT 5,
    PauseBetweenWaterMin INT NOT NULL DEFAULT 2,
    WaterRetention NVARCHAR(20) NULL,
    DrainageRate NVARCHAR(20) NULL,

    -- Composition
    PHLevel FLOAT NULL,
    Composition NVARCHAR(500) NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- =============================================
-- IOT DEVICE MANAGEMENT
-- =============================================

-- Devices Table
CREATE TABLE Devices (
    DeviceId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PlantId INT NULL,

    -- Device identification
    DeviceName NVARCHAR(100) NULL,
    DeviceToken NVARCHAR(255) NOT NULL UNIQUE,
    MacAddress NVARCHAR(17) NOT NULL UNIQUE,
    IpAddress NVARCHAR(45) NULL,
    FirmwareVersion NVARCHAR(20) NULL,
    Model NVARCHAR(50) NULL,
    SerialNumber NVARCHAR(50) NULL,

    -- Device status
    IsOnline BIT NOT NULL DEFAULT 0,
    LastSeen DATETIME2 NULL,
    LastHeartbeat DATETIME2 NULL,
    BatteryLevel FLOAT NULL,
    SignalStrength INT NULL,

    -- Configuration
    ReadingIntervalSec INT NOT NULL DEFAULT 900,
    IsCalibrated BIT NOT NULL DEFAULT 0,
    CalibrationDate DATETIME2 NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Devices_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Devices_UserId ON Devices(UserId);
CREATE INDEX IX_Devices_PlantId ON Devices(PlantId);
CREATE INDEX IX_Devices_MacAddress ON Devices(MacAddress);
CREATE INDEX IX_Devices_DeviceToken ON Devices(DeviceToken);
CREATE INDEX IX_Devices_IsOnline ON Devices(IsOnline);
CREATE INDEX IX_Devices_LastSeen ON Devices(LastSeen);
GO

-- DeviceCommands Table
CREATE TABLE DeviceCommands (
    CommandId BIGINT IDENTITY(1,1) PRIMARY KEY,
    DeviceId INT NOT NULL,
    UserId INT NULL,

    -- Command details
    CommandType INT NOT NULL,
    CommandPayload NVARCHAR(MAX) NULL,

    -- Command status
    Status INT NOT NULL DEFAULT 0,
    SentAt DATETIME2 NULL,
    AcknowledgedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,

    -- Result
    ResponsePayload NVARCHAR(MAX) NULL,
    ErrorMessage NVARCHAR(500) NULL,

    -- Timeout
    TimeoutAt DATETIME2 NULL,
    RetryCount INT NOT NULL DEFAULT 0,
    MaxRetries INT NOT NULL DEFAULT 3,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_DeviceCommands_Devices FOREIGN KEY (DeviceId)
        REFERENCES Devices(DeviceId) ON DELETE CASCADE,
    CONSTRAINT FK_DeviceCommands_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_DeviceCommands_DeviceId ON DeviceCommands(DeviceId);
CREATE INDEX IX_DeviceCommands_Status ON DeviceCommands(Status);
CREATE INDEX IX_DeviceCommands_Device_Status ON DeviceCommands(DeviceId, Status);
CREATE INDEX IX_DeviceCommands_CreatedAt ON DeviceCommands(CreatedAt);
GO

-- =============================================
-- PLANT MANAGEMENT
-- =============================================

-- Plants Table
CREATE TABLE Plants (
    PlantId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    SpeciesId INT NOT NULL,
    SoilTypeId INT NOT NULL,

    -- Plant-specific info
    Nickname NVARCHAR(80) NULL,
    RoomName NVARCHAR(80) NULL,
    IsOutdoor BIT NOT NULL DEFAULT 0,
    DateAcquired DATETIME2 NULL,

    -- Plant status
    CurrentHealthStatus INT NULL,
    CurrentHealthScore FLOAT NULL,
    Notes NVARCHAR(MAX) NULL,

    -- Last known readings (denormalized)
    LastSoilMoisture FLOAT NULL,
    LastAirTemp FLOAT NULL,
    LastAirHumidity FLOAT NULL,
    LastReadingAt DATETIME2 NULL,

    -- Last watering
    LastWateredAt DATETIME2 NULL,
    LastWateredBy NVARCHAR(20) NULL,

    -- Shadow properties
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
CREATE INDEX IX_Plants_HealthStatus ON Plants(CurrentHealthStatus);
CREATE INDEX IX_Plants_UserId_SpeciesId ON Plants(UserId, SpeciesId);
CREATE INDEX IX_Plants_UserId_IsOutdoor ON Plants(UserId, IsOutdoor);
CREATE INDEX IX_Plants_UserId_Health ON Plants(UserId, CurrentHealthStatus);
GO

-- Add FK from Devices to Plants (after Plants table exists)
ALTER TABLE Devices
ADD CONSTRAINT FK_Devices_Plants FOREIGN KEY (PlantId)
    REFERENCES Plants(PlantId) ON DELETE NO ACTION;
GO

-- PlantThresholds Table
CREATE TABLE PlantThresholds (
    PlantId INT PRIMARY KEY,

    -- Custom environmental thresholds
    CustomSoilMoistMin FLOAT NULL,
    CustomSoilMoistMax FLOAT NULL,
    CustomTempMin FLOAT NULL,
    CustomTempMax FLOAT NULL,
    CustomLightMin FLOAT NULL,
    CustomLightMax FLOAT NULL,
    CustomHumidityMin FLOAT NULL,
    CustomHumidityMax FLOAT NULL,

    -- Custom watering settings
    CustomWaterDurationSec INT NULL,
    CustomWaterThreshold FLOAT NULL,
    CustomWaterFrequencyDays INT NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_PlantThresholds_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE
);
GO

-- PlantHealth Table
CREATE TABLE PlantHealth (
    HealthId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,

    -- Health metrics
    HealthScore FLOAT NOT NULL,
    HealthStatus INT NOT NULL,

    -- Contributing factors
    MoistureScore FLOAT NOT NULL,
    TemperatureScore FLOAT NOT NULL,
    HumidityScore FLOAT NOT NULL,
    LightScore FLOAT NOT NULL,

    -- Analysis
    Issues NVARCHAR(MAX) NULL,
    Recommendations NVARCHAR(MAX) NULL,

    -- Calculation metadata
    DataPointsAnalyzed INT NOT NULL,
    AnalysisPeriodHours INT NOT NULL DEFAULT 24,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_PlantHealth_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_PlantHealth_PlantId ON PlantHealth(PlantId);
CREATE INDEX IX_PlantHealth_Status ON PlantHealth(HealthStatus);
CREATE INDEX IX_PlantHealth_CreatedAt ON PlantHealth(CreatedAt);
CREATE INDEX IX_PlantHealth_Plant_Created ON PlantHealth(PlantId, CreatedAt);
GO

-- PlantPhotos Table
CREATE TABLE PlantPhotos (
    PhotoId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    UserId INT NOT NULL,

    -- Photo details
    ImageUrl NVARCHAR(500) NOT NULL,
    ThumbnailUrl NVARCHAR(500) NULL,
    Caption NVARCHAR(200) NULL,

    -- Plant measurements
    HeightCm FLOAT NULL,
    LeafCount INT NULL,

    -- Photo metadata
    FileSize INT NULL,
    MimeType NVARCHAR(50) NULL,
    Width INT NULL,
    Height INT NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_PlantPhotos_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,
    CONSTRAINT FK_PlantPhotos_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_PlantPhotos_PlantId ON PlantPhotos(PlantId);
CREATE INDEX IX_PlantPhotos_UserId ON PlantPhotos(UserId);
CREATE INDEX IX_PlantPhotos_CreatedAt ON PlantPhotos(CreatedAt);
GO

-- =============================================
-- SENSOR DATA & WATERING
-- =============================================

-- SensorReadings Table
CREATE TABLE SensorReadings (
    ReadingId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    DeviceId INT NULL,

    -- Sensor data from ESP32
    SoilMoisture FLOAT NOT NULL,
    AirTemp FLOAT NOT NULL,
    AirHumidity FLOAT NOT NULL,
    LightLevel FLOAT NOT NULL DEFAULT 0,
    AirQuality FLOAT NOT NULL DEFAULT 0,
    WaterLevel FLOAT NOT NULL DEFAULT 0,

    -- Raw sensor values
    RawSoilMoisture INT NULL,
    RawAirTemp INT NULL,

    -- Reading metadata
    ReadingDurationMs INT NULL,
    BatteryVoltage FLOAT NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_SensorReadings_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,
    CONSTRAINT FK_SensorReadings_Devices FOREIGN KEY (DeviceId)
        REFERENCES Devices(DeviceId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_SensorReadings_PlantId ON SensorReadings(PlantId);
CREATE INDEX IX_SensorReadings_DeviceId ON SensorReadings(DeviceId);
CREATE INDEX IX_SensorReadings_CreatedAt ON SensorReadings(CreatedAt DESC);
CREATE INDEX IX_SensorReadings_Plant_Created ON SensorReadings(PlantId, CreatedAt);
GO

-- WateringSchedules Table
CREATE TABLE WateringSchedules (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,

    -- Schedule configuration
    IsEnabled BIT NOT NULL DEFAULT 1,
    ScheduleName NVARCHAR(100) NULL,
    ScheduleType INT NOT NULL DEFAULT 0,

    -- Timing
    TimeOfDay TIME NOT NULL,
    DaysOfWeek NVARCHAR(20) NULL,
    IntervalDays INT NULL,

    -- Watering details
    DurationSec INT NOT NULL DEFAULT 5,

    -- Schedule state
    NextRunAt DATETIME2 NULL,
    LastRunAt DATETIME2 NULL,

    -- Statistics
    TotalRuns INT NOT NULL DEFAULT 0,
    SuccessfulRuns INT NOT NULL DEFAULT 0,
    FailedRuns INT NOT NULL DEFAULT 0,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_WateringSchedules_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_WateringSchedules_PlantId ON WateringSchedules(PlantId);
CREATE INDEX IX_WateringSchedules_IsEnabled ON WateringSchedules(IsEnabled);
CREATE INDEX IX_WateringSchedules_NextRunAt ON WateringSchedules(NextRunAt);
CREATE INDEX IX_WateringSchedules_Enabled_NextRun ON WateringSchedules(IsEnabled, NextRunAt);
GO

-- WateringLogs Table
CREATE TABLE WateringLogs (
    WateringId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    DeviceId INT NULL,
    UserId INT NULL,
    ScheduleId INT NULL,

    -- Watering details
    DurationSec INT NOT NULL DEFAULT 5,
    Mode INT NOT NULL DEFAULT 0,

    -- Pre-watering conditions
    SoilMoistureBeforeWater FLOAT NULL,
    SoilMoistureAfterWater FLOAT NULL,

    -- Watering result
    WasSuccessful BIT NOT NULL DEFAULT 1,
    ErrorMessage NVARCHAR(500) NULL,
    Notes NVARCHAR(500) NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_WateringLogs_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,
    CONSTRAINT FK_WateringLogs_Devices FOREIGN KEY (DeviceId)
        REFERENCES Devices(DeviceId) ON DELETE NO ACTION,
    CONSTRAINT FK_WateringLogs_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_WateringLogs_Schedules FOREIGN KEY (ScheduleId)
        REFERENCES WateringSchedules(ScheduleId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_WateringLogs_PlantId ON WateringLogs(PlantId);
CREATE INDEX IX_WateringLogs_DeviceId ON WateringLogs(DeviceId);
CREATE INDEX IX_WateringLogs_UserId ON WateringLogs(UserId);
CREATE INDEX IX_WateringLogs_ScheduleId ON WateringLogs(ScheduleId);
CREATE INDEX IX_WateringLogs_Mode ON WateringLogs(Mode);
CREATE INDEX IX_WateringLogs_CreatedAt ON WateringLogs(CreatedAt DESC);
CREATE INDEX IX_WateringLogs_Plant_Created ON WateringLogs(PlantId, CreatedAt);
GO

-- =============================================
-- ALERTS & NOTIFICATIONS
-- =============================================

-- Alerts Table
CREATE TABLE Alerts (
    AlertId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    UserId INT NOT NULL,
    DeviceId INT NULL,

    -- Alert details
    AlertType INT NOT NULL,
    Severity INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(500) NOT NULL,

    -- Alert metadata
    TriggerValue FLOAT NULL,
    ThresholdValue FLOAT NULL,
    ReadingId BIGINT NULL,

    -- Alert status
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME2 NULL,
    IsResolved BIT NOT NULL DEFAULT 0,
    ResolvedAt DATETIME2 NULL,
    ResolvedByUserId INT NULL,

    -- Notification tracking
    EmailSent BIT NOT NULL DEFAULT 0,
    PushSent BIT NOT NULL DEFAULT 0,
    SMSSent BIT NOT NULL DEFAULT 0,

    -- Auto-dismiss
    AutoDismissAt DATETIME2 NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Alerts_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,
    CONSTRAINT FK_Alerts_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Alerts_Devices FOREIGN KEY (DeviceId)
        REFERENCES Devices(DeviceId) ON DELETE NO ACTION,
    CONSTRAINT FK_Alerts_Readings FOREIGN KEY (ReadingId)
        REFERENCES SensorReadings(ReadingId) ON DELETE NO ACTION,
    CONSTRAINT FK_Alerts_ResolvedBy FOREIGN KEY (ResolvedByUserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Alerts_PlantId ON Alerts(PlantId);
CREATE INDEX IX_Alerts_UserId ON Alerts(UserId);
CREATE INDEX IX_Alerts_DeviceId ON Alerts(DeviceId);
CREATE INDEX IX_Alerts_AlertType ON Alerts(AlertType);
CREATE INDEX IX_Alerts_Severity ON Alerts(Severity);
CREATE INDEX IX_Alerts_IsRead ON Alerts(IsRead);
CREATE INDEX IX_Alerts_IsResolved ON Alerts(IsResolved);
CREATE INDEX IX_Alerts_CreatedAt ON Alerts(CreatedAt);
CREATE INDEX IX_Alerts_User_Status ON Alerts(UserId, IsRead, IsResolved);
GO

-- NotificationSettings Table
CREATE TABLE NotificationSettings (
    NotificationSettingId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,

    -- Notification channel
    Channel INT NOT NULL,

    -- Alert type preferences
    EnableLowMoisture BIT NOT NULL DEFAULT 1,
    EnableHighMoisture BIT NOT NULL DEFAULT 1,
    EnableTemperature BIT NOT NULL DEFAULT 1,
    EnableHumidity BIT NOT NULL DEFAULT 1,
    EnableLight BIT NOT NULL DEFAULT 0,
    EnableWaterLevel BIT NOT NULL DEFAULT 1,
    EnableDeviceOffline BIT NOT NULL DEFAULT 1,
    EnableMaintenance BIT NOT NULL DEFAULT 1,

    -- Severity filter
    MinimumSeverity INT NOT NULL DEFAULT 1,

    -- Channel-specific settings
    ChannelAddress NVARCHAR(255) NULL,
    IsVerified BIT NOT NULL DEFAULT 0,

    -- Rate limiting
    MaxNotificationsPerHour INT NOT NULL DEFAULT 10,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_NotificationSettings_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_NotificationSettings_UserId ON NotificationSettings(UserId);
CREATE INDEX IX_NotificationSettings_Channel ON NotificationSettings(Channel);
CREATE INDEX IX_NotificationSettings_User_Channel ON NotificationSettings(UserId, Channel);
GO

-- =============================================
-- MAINTENANCE & CARE
-- =============================================

-- MaintenanceLogs Table
CREATE TABLE MaintenanceLogs (
    MaintenanceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    PlantId INT NOT NULL,
    UserId INT NOT NULL,

    -- Maintenance details
    MaintenanceType INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,

    -- Scheduling
    PerformedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    NextDueDate DATETIME2 NULL,
    FrequencyDays INT NULL,

    -- Products used
    ProductsUsed NVARCHAR(500) NULL,
    Cost DECIMAL(10,2) NULL,

    -- Photos
    BeforePhotoUrl NVARCHAR(500) NULL,
    AfterPhotoUrl NVARCHAR(500) NULL,

    -- Reminders
    SendReminderDaysBefore INT NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_MaintenanceLogs_Plants FOREIGN KEY (PlantId)
        REFERENCES Plants(PlantId) ON DELETE CASCADE,
    CONSTRAINT FK_MaintenanceLogs_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_MaintenanceLogs_PlantId ON MaintenanceLogs(PlantId);
CREATE INDEX IX_MaintenanceLogs_UserId ON MaintenanceLogs(UserId);
CREATE INDEX IX_MaintenanceLogs_Type ON MaintenanceLogs(MaintenanceType);
CREATE INDEX IX_MaintenanceLogs_NextDue ON MaintenanceLogs(NextDueDate);
CREATE INDEX IX_MaintenanceLogs_PerformedAt ON MaintenanceLogs(PerformedAt);
GO

-- =============================================
-- SYSTEM & SECURITY
-- =============================================

-- AuditLogs Table
CREATE TABLE AuditLogs (
    AuditId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,

    -- Action details
    Action NVARCHAR(100) NOT NULL,
    EntityType NVARCHAR(50) NULL,
    EntityId NVARCHAR(50) NULL,

    -- Request metadata
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,

    -- Change tracking
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,

    -- Result
    Success BIT NOT NULL DEFAULT 1,
    ErrorMessage NVARCHAR(500) NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
CREATE INDEX IX_AuditLogs_EntityType ON AuditLogs(EntityType);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
CREATE INDEX IX_AuditLogs_User_Created ON AuditLogs(UserId, CreatedAt);
GO

-- SystemLogs Table
CREATE TABLE SystemLogs (
    LogId BIGINT IDENTITY(1,1) PRIMARY KEY,

    -- Log details
    Level NVARCHAR(20) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Exception NVARCHAR(MAX) NULL,

    -- Context
    Source NVARCHAR(100) NULL,
    UserId INT NULL,
    DeviceId INT NULL,

    -- Request tracking
    RequestId NVARCHAR(100) NULL,
    IpAddress NVARCHAR(45) NULL,

    -- Additional data
    Properties NVARCHAR(MAX) NULL,

    -- Shadow properties
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_SystemLogs_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_SystemLogs_Devices FOREIGN KEY (DeviceId)
        REFERENCES Devices(DeviceId) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_SystemLogs_Level ON SystemLogs(Level);
CREATE INDEX IX_SystemLogs_Source ON SystemLogs(Source);
CREATE INDEX IX_SystemLogs_UserId ON SystemLogs(UserId);
CREATE INDEX IX_SystemLogs_DeviceId ON SystemLogs(DeviceId);
CREATE INDEX IX_SystemLogs_CreatedAt ON SystemLogs(CreatedAt);
CREATE INDEX IX_SystemLogs_Level_Created ON SystemLogs(Level, CreatedAt);
GO

-- =============================================
-- EF Core Migration History Table
-- =============================================

CREATE TABLE __EFMigrationsHistory (
    MigrationId NVARCHAR(150) PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
);
GO

-- Insert existing migrations
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES
    ('20250806134551_InitialCreate', '9.0.8'),
    ('20250806143925_AddPlantRelations', '9.0.8');
GO

-- =============================================
-- Seed Data for Lookup Tables
-- =============================================

-- Insert default Species
INSERT INTO Species (CommonName, ScientificName, Description, DefaultSoilMoistMin, DefaultSoilMoistMax, DefaultTempMin, DefaultTempMax, DefaultLightMin, DefaultLightMax, DefaultHumidityMin, DefaultHumidityMax, DefaultWaterFrequencyDays, Category, Tags, GrowthRate, MaxHeightCm)
VALUES
    ('Basil', 'Ocimum basilicum', 'Popular aromatic herb for cooking', 40.0, 70.0, 18.0, 27.0, 6000.0, 15000.0, 50.0, 70.0, 2, 'Herb', 'edible,aromatic,culinary', 'Fast', 60),
    ('Tomato', 'Solanum lycopersicum', 'Productive fruiting vegetable', 40.0, 80.0, 20.0, 30.0, 8000.0, 20000.0, 60.0, 80.0, 3, 'Vegetable', 'edible,fruit-bearing', 'Fast', 200),
    ('Lettuce', 'Lactuca sativa', 'Quick-growing leafy green', 50.0, 75.0, 15.0, 20.0, 4000.0, 10000.0, 60.0, 80.0, 2, 'Vegetable', 'edible,leafy', 'Fast', 30),
    ('Mint', 'Mentha', 'Hardy aromatic herb', 50.0, 80.0, 15.0, 25.0, 3000.0, 8000.0, 60.0, 85.0, 2, 'Herb', 'aromatic,medicinal,invasive', 'Fast', 40),
    ('Cactus', 'Cactaceae', 'Low-maintenance succulent', 10.0, 30.0, 20.0, 35.0, 10000.0, 30000.0, 20.0, 40.0, 14, 'Succulent', 'drought-tolerant,decorative', 'Slow', 100),
    ('Fern', 'Polypodiopsida', 'Shade-loving houseplant', 60.0, 90.0, 18.0, 24.0, 1000.0, 5000.0, 70.0, 90.0, 3, 'Houseplant', 'shade-loving,humidity-loving', 'Medium', 60),
    ('Spider Plant', 'Chlorophytum comosum', 'Easy-care air purifying plant', 40.0, 60.0, 18.0, 27.0, 3000.0, 10000.0, 40.0, 70.0, 4, 'Houseplant', 'air-purifying,beginner-friendly', 'Fast', 40),
    ('Pothos', 'Epipremnum aureum', 'Trailing vine houseplant', 40.0, 60.0, 18.0, 29.0, 2000.0, 8000.0, 50.0, 70.0, 5, 'Houseplant', 'air-purifying,low-light,trailing', 'Fast', 300),
    ('Snake Plant', 'Sansevieria trifasciata', 'Nearly indestructible succulent', 20.0, 40.0, 18.0, 29.0, 2000.0, 10000.0, 30.0, 50.0, 10, 'Succulent', 'air-purifying,drought-tolerant,low-maintenance', 'Slow', 120),
    ('Aloe Vera', 'Aloe barbadensis miller', 'Medicinal succulent', 15.0, 35.0, 18.0, 30.0, 8000.0, 20000.0, 30.0, 50.0, 10, 'Succulent', 'medicinal,drought-tolerant,healing', 'Slow', 60);
GO

-- Insert default SoilTypes
INSERT INTO SoilTypes (Name, Description, RecWaterDueSec, PauseBetweenWaterMin, WaterRetention, DrainageRate, PHLevel, Composition)
VALUES
    ('Potting Mix', 'Standard indoor potting soil with good drainage and moisture retention', 5, 2, 'Medium', 'Good', 6.5, 'Peat moss, perlite, vermiculite'),
    ('Cactus Mix', 'Sandy, well-draining soil for succulents and cacti', 3, 5, 'Low', 'Excellent', 6.0, 'Sand, perlite, pumice, minimal organics'),
    ('Peat Moss', 'High water retention, acidic soil for moisture-loving plants', 7, 1, 'High', 'Poor', 4.5, '100% sphagnum peat moss'),
    ('Clay Soil', 'Heavy soil with poor drainage, needs less frequent watering', 8, 4, 'High', 'Poor', 7.0, 'Fine clay particles, minimal organics'),
    ('Sandy Soil', 'Light soil with excellent drainage, needs frequent watering', 4, 1, 'Low', 'Excellent', 6.5, 'Coarse sand, minimal clay'),
    ('Loamy Soil', 'Balanced mix of sand, silt, and clay - ideal for most plants', 5, 2, 'Medium', 'Good', 6.8, 'Balanced sand, silt, clay, organic matter'),
    ('Orchid Bark', 'Chunky bark mix for orchids and epiphytes', 3, 3, 'Low', 'Excellent', 6.0, 'Bark chunks, charcoal, perlite'),
    ('Seed Starting Mix', 'Fine, sterile mix for germinating seeds', 4, 2, 'Medium', 'Good', 6.5, 'Fine peat, vermiculite, perlite');
GO

-- =============================================
-- Summary
-- =============================================

PRINT '===============================================';
PRINT '✅ SmartGarden Database Created Successfully!';
PRINT '===============================================';
PRINT '';
PRINT '📊 Database: SmartGardenDB';
PRINT '';
PRINT '📋 Tables Created: 18';
PRINT '   Core (2): Users, UserSettings';
PRINT '   Master Data (2): Species, SoilTypes';
PRINT '   IoT Devices (2): Devices, DeviceCommands';
PRINT '   Plants (4): Plants, PlantThresholds, PlantHealth, PlantPhotos';
PRINT '   Sensors & Water (3): SensorReadings, WateringLogs, WateringSchedules';
PRINT '   Alerts (2): Alerts, NotificationSettings';
PRINT '   Maintenance (1): MaintenanceLogs';
PRINT '   System (2): AuditLogs, SystemLogs';
PRINT '';
PRINT '🌱 Seed Data Inserted:';
PRINT '   - 10 Plant Species';
PRINT '   - 8 Soil Types';
PRINT '';
PRINT '🔗 Total Foreign Keys: 20+';
PRINT '📊 Total Indexes: 80+';
PRINT '📝 Total Columns: 200+';
PRINT '';
PRINT '✨ Ready to use with SmartGarden API!';
PRINT '===============================================';
GO
