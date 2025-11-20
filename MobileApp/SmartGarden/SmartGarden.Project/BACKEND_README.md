# 🌱 SmartGarden Backend API

Complete .NET 8.0 REST API for IoT plant monitoring and automated watering system.

---

## 📋 Table of Contents

- [Features](#features)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [Testing](#testing)

---

## ✨ Features

### Core Functionality
- ✅ **User Authentication** - JWT-based authentication with refresh tokens
- ✅ **Device Management** - ESP32 device registration and approval workflow
- ✅ **Plant Management** - Full CRUD operations for plants with species and soil types
- ✅ **Sensor Data** - Real-time sensor readings from IoT devices
- ✅ **Watering Control** - Manual and automatic watering with scheduling
- ✅ **Alerts & Notifications** - Plant health monitoring and alert system

### Security Features
- ✅ **Dual JWT Authentication** - Separate tokens for users and devices
- ✅ **HMAC-SHA256 Signing** - Message integrity verification for sensor data
- ✅ **API Key Hashing** - SHA256 hashing for device API keys
- ✅ **Rate Limiting** - Per-device and per-endpoint rate limits
- ✅ **Device Approval Workflow** - Manual approval required for new devices
- ✅ **Failed Auth Tracking** - Automatic lockout after failed attempts
- ✅ **CORS Configuration** - Configurable allowed origins

### Technical Features
- ✅ **Entity Framework Core** - Code-first database with migrations
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Soft Delete** - Audit trails with soft delete support
- ✅ **Background Services** - Automated watering scheduler
- ✅ **Repository Pattern** - Clean architecture with dependency injection

---

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0 SDK** or higher ([Download](https://dotnet.microsoft.com/download))
- **SQL Server** (Express/LocalDB/Full) ([Download](https://www.microsoft.com/sql-server/sql-server-downloads))

### Option 1: Automated Setup (Recommended)

**Linux/macOS:**
```bash
./setup-backend.sh
./start-backend.sh
```

**Windows:**
```cmd
setup-backend.bat
start-backend.bat
```

### Option 2: Manual Setup

**Step 1: Restore Dependencies**
```bash
dotnet restore
```

**Step 2: Configure Database**

Edit `SmartGarden.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartGardenDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Step 3: Create Database**
```bash
# Create migration
dotnet ef migrations add AddDeviceAuthAndSecurityFeatures \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API

# Apply migration
dotnet ef database update \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API
```

**Step 4: Run API**
```bash
cd SmartGarden.API
dotnet run
```

**Step 5: Access API**
- **Swagger UI:** https://localhost:5000
- **HTTPS:** https://localhost:5000/api
- **HTTP:** http://localhost:5001/api

---

## 📁 Project Structure

```
SmartGarden.Project/
├── SmartGarden.API/              # REST API & Controllers
│   ├── Controllers/              # API endpoints (8 controllers)
│   │   ├── AuthController.cs           # User auth (login, register)
│   │   ├── DeviceAuthController.cs     # Device registration & approval
│   │   ├── PlantController.cs          # Plant CRUD operations
│   │   ├── SensorController.cs         # Sensor data management
│   │   ├── WateringController.cs       # Watering control
│   │   ├── DeviceController.cs         # Device management
│   │   ├── AlertController.cs          # Alert system
│   │   └── HomeController.cs           # Health check
│   ├── Services/                 # Business logic services
│   │   ├── AuthService.cs              # Authentication logic
│   │   ├── DeviceAuthService.cs        # Device security
│   │   ├── PlantService.cs             # Plant management
│   │   ├── SensorService.cs            # Sensor data processing
│   │   ├── WateringService.cs          # Watering logic
│   │   ├── DeviceService.cs            # Device operations
│   │   ├── AlertService.cs             # Alert notifications
│   │   └── AutoWateringBackgroundService.cs  # Background scheduler
│   ├── Program.cs                # App configuration & DI
│   └── appsettings.json          # Configuration
│
├── SmartGarden.Core/             # Domain models & interfaces
│   ├── Models/                   # Entity models (18 entities)
│   │   ├── User.cs, Plant.cs, Species.cs
│   │   ├── Device.cs, DeviceAuth.cs, DeviceCommand.cs
│   │   ├── SensorReading.cs, WateringLog.cs, WateringSchedule.cs
│   │   ├── Alert.cs, PlantHealth.cs, PlantThreshold.cs
│   │   └── ... (and more)
│   ├── DTOs/                     # Data transfer objects
│   └── Interfaces/               # Service contracts
│
└── SmartGarden.Data/             # Data access layer
    ├── Persistence/
    │   ├── SmartGardenDbContext.cs      # EF Core context
    │   └── Configurations/              # Fluent API configs
    └── Migrations/               # Database migrations
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartGardenDB;Integrated Security=true;TrustServerCertificate=true;"
  },

  "JwtSettings": {
    "UserSecret": "YOUR-SECURE-SECRET-KEY-FOR-USERS-MIN-32-CHARS",
    "DeviceSecret": "YOUR-SECURE-SECRET-KEY-FOR-DEVICES-MIN-32-CHARS",
    "Issuer": "SmartGarden",
    "Audience": "SmartGarden",
    "ExpirationDays": 7
  },

  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",     // React web app
      "http://localhost:19000",    // Expo dev server
      "http://localhost:19006"     // Expo web
    ]
  },

  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      },
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "1m",
        "Limit": 5
      }
    ]
  }
}
```

### Database Connection Strings

**SQL Server LocalDB (Windows):**
```json
"Server=(localdb)\\mssqllocaldb;Database=SmartGardenDB;Trusted_Connection=True;TrustServerCertificate=True"
```

**SQL Server Express:**
```json
"Server=localhost\\SQLEXPRESS;Database=SmartGardenDB;Integrated Security=true;TrustServerCertificate=true;"
```

**SQL Server with credentials:**
```json
"Server=localhost;Database=SmartGardenDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
```

---

## 🗄️ Database Setup

### Database Schema

The system creates **18 tables**:

| Category | Tables |
|----------|--------|
| **Users** | Users, UserSettings, NotificationSettings |
| **Plants** | Plants, Species, SoilTypes, PlantThresholds, PlantHealths, PlantPhotos |
| **Devices** | Devices, DeviceAuths, DeviceCommands |
| **Sensors** | SensorReadings |
| **Watering** | WateringLogs, WateringSchedules |
| **System** | Alerts, MaintenanceLogs, AuditLogs, SystemLogs |

### Key Tables

**DeviceAuths** - Security credentials for ESP32 devices
```sql
CREATE TABLE DeviceAuths (
    DeviceAuthId INT PRIMARY KEY,
    DeviceId INT NOT NULL UNIQUE,
    ApiKeyHash NVARCHAR(256) NOT NULL,
    RefreshToken NVARCHAR(500) NOT NULL,
    TokenExpiry DATETIME2,
    IsApproved BIT DEFAULT 0,
    RequestCount INT DEFAULT 0,
    FailedAuthAttempts INT DEFAULT 0,
    IsLocked BIT DEFAULT 0,
    ...
);
```

**SensorReadings** - Time-series sensor data
```sql
CREATE TABLE SensorReadings (
    ReadingId BIGINT PRIMARY KEY,
    PlantId INT NOT NULL,
    DeviceId INT NOT NULL,
    SoilMoisture FLOAT,
    AirTemperature FLOAT,
    AirHumidity FLOAT,
    LightLevel FLOAT,
    WaterLevel FLOAT,
    Timestamp DATETIME2,
    ...
);
```

### Migrations

**Create new migration:**
```bash
dotnet ef migrations add YourMigrationName \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API
```

**Apply migrations:**
```bash
dotnet ef database update \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API
```

**Rollback migration:**
```bash
dotnet ef database update PreviousMigrationName \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API
```

---

## 🌐 API Endpoints

### Authentication (User)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register new user | None |
| POST | `/api/auth/login` | User login | None |
| GET | `/api/auth/profile` | Get user profile | User |
| POST | `/api/auth/refresh-token` | Refresh JWT token | None |
| GET | `/api/auth/exists` | Check if email exists | None |

**Example Request:**
```bash
curl -X POST https://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "john_doe",
  "email": "user@example.com"
}
```

---

### Device Authentication

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/device-auth/register` | Register ESP32 device | None |
| POST | `/api/device-auth/approve` | Approve device | User |
| GET | `/api/device-auth/pending` | Get pending devices | User |
| POST | `/api/device-auth/heartbeat` | Device heartbeat | Device |
| POST | `/api/device-auth/refresh-token` | Refresh device token | Device |

**Device Registration:**
```bash
curl -X POST https://localhost:5000/api/device-auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "macAddress": "AA:BB:CC:DD:EE:FF",
    "model": "ESP32-SmartGarden-v1",
    "firmwareVersion": "1.0.0"
  }'
```

**Response:**
```json
{
  "deviceId": 1,
  "deviceToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "apiKey": "base64-encoded-key",
  "refreshToken": "refresh-token-string",
  "requiresApproval": true
}
```

---

### Plant Management

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/plants` | Get all user plants | User |
| GET | `/api/plants/{id}` | Get plant by ID | User |
| POST | `/api/plants` | Create new plant | User |
| PUT | `/api/plants/{id}` | Update plant | User |
| DELETE | `/api/plants/{id}` | Delete plant (soft) | User |

---

### Sensor Data

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/sensor` | Post sensor reading | Device |
| GET | `/api/sensor/plant/{id}/latest` | Get latest reading | User |
| GET | `/api/sensor/plant/{id}/history` | Get history | User |
| GET | `/api/sensor/plant/{id}/statistics` | Get statistics | User |

**Post Sensor Data:**
```bash
curl -X POST https://localhost:5000/api/sensor \
  -H "Authorization: Bearer DEVICE_TOKEN" \
  -H "X-Device-ID: 1" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 1,
    "plantId": 1,
    "soilMoisture": 45.2,
    "airTemperature": 22.5,
    "airHumidity": 55.0,
    "lightLevel": 320.0,
    "waterLevel": 75.0,
    "timestamp": 1234567890,
    "signature": "hmac-sha256-signature"
  }'
```

---

### Watering Control

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/watering/manual` | Trigger manual watering | User |
| GET | `/api/watering/plant/{id}/commands` | Get watering commands | Device |
| PUT | `/api/watering/plant/{id}/auto` | Configure auto-watering | User |

---

## 🔐 Security

### Authentication Flow

```
┌─────────────────────────────────────────────┐
│          DUAL AUTHENTICATION SYSTEM          │
└─────────────────────────────────────────────┘

USER FLOW:
1. User registers → POST /api/auth/register
2. User logs in → POST /api/auth/login
3. Receive JWT token (UserSecret, 7 days)
4. Use token in Authorization header
5. Refresh before expiry → POST /api/auth/refresh-token

DEVICE FLOW:
1. ESP32 registers → POST /api/device-auth/register
2. Receive device token + API key (DeviceSecret, 24h)
3. Wait for approval → IsApproved = false
4. User approves → POST /api/device-auth/approve
5. Send heartbeat → POST /api/device-auth/heartbeat (every 1 min)
6. Post data with HMAC → POST /api/sensor (with signature)
7. Auto-refresh token → POST /api/device-auth/refresh-token
```

### HMAC Signature Verification

**ESP32 creates signature:**
```cpp
String payload = "{\"deviceId\":1,\"soilMoisture\":45.2,...}";
String signature = hmacSHA256(payload, apiKey);
```

**Backend verifies signature:**
```csharp
var computedSignature = ComputeHmacSha256(payload, apiKey);
if (receivedSignature != computedSignature) {
    return Unauthorized("Invalid signature");
}
```

### Rate Limiting

| Endpoint | Limit | Period |
|----------|-------|--------|
| All endpoints | 60 requests | 1 minute |
| All endpoints | 1000 requests | 1 hour |
| POST /api/auth/login | 5 requests | 1 minute |
| POST /api/auth/register | 3 requests | 1 hour |
| Device endpoints | 120 requests | 1 hour |

### Device Lockout

- **3 failed auth attempts** → Device locked for 30 minutes
- **Reset on successful auth**
- **Manual unlock** via database or API

---

## 🧪 Testing

### Swagger UI

1. Navigate to: https://localhost:5000
2. Click "Authorize" button
3. Enter: `Bearer YOUR_JWT_TOKEN`
4. Test endpoints interactively

### API Testing with cURL

**Health Check:**
```bash
curl -k https://localhost:5000/api/home
```

**User Registration:**
```bash
curl -k -X POST https://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "username": "testuser"
  }'
```

**Get Plants (requires auth):**
```bash
curl -k https://localhost:5000/api/plants \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Database Verification

**Connect to database:**
```bash
sqlcmd -S localhost -d SmartGardenDB -E
```

**Check tables:**
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE='BASE TABLE';
```

**Verify user registration:**
```sql
SELECT * FROM Users;
```

---

## 🐛 Troubleshooting

### "Connection failed" errors

**Check SQL Server is running:**
```bash
# Windows
sc query MSSQLSERVER

# Check connection string in appsettings.json
```

### "Migration failed" errors

**Delete and recreate migrations:**
```bash
# Remove migration
dotnet ef migrations remove --project SmartGarden.Data --startup-project SmartGarden.API

# Recreate
dotnet ef migrations add InitialCreate --project SmartGarden.Data --startup-project SmartGarden.API
```

### Port already in use

**Change port in launchSettings.json:**
```json
"applicationUrl": "https://localhost:5001;http://localhost:5002"
```

### CORS errors from frontend

**Add origin to appsettings.json:**
```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://your-frontend-url:port"
  ]
}
```

---

## 📚 Additional Resources

- **Full System Documentation:** `/COMPLETE_SYSTEM_SUMMARY.md`
- **Implementation Guide:** `/IMPLEMENTATION_GUIDE.md`
- **Security Features:** `/SECURITY_FEATURES.md`
- **ESP32 Firmware:** `/FirmWare/SecureESP32/README.md`
- **React App:** `/ReactNativeApp/SmartGardenApp/`

---

## 📞 Support

For issues:
1. Check logs in console output
2. Verify database connection
3. Review this documentation
4. Check Swagger UI for API details

---

**Last Updated:** 2025-11-20
**Version:** 1.0.0
**License:** MIT
