# SmartGarden - Project Documentation

**Version:** 2.0
**Last Updated:** November 2025
**Framework:** .NET 10 | React 18
**License:** MIT

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [System Overview](#system-overview)
3. [Architecture](#architecture)
4. [Technology Stack](#technology-stack)
5. [Database Design](#database-design)
6. [Backend API](#backend-api)
7. [Frontend Application](#frontend-application)
8. [Security](#security)
9. [Real-Time Communication](#real-time-communication)
10. [IoT Integration](#iot-integration)
11. [Installation & Setup](#installation--setup)
12. [Configuration](#configuration)
13. [Development Guide](#development-guide)
14. [Testing](#testing)
15. [Deployment](#deployment)
16. [Performance Optimization](#performance-optimization)
17. [Monitoring & Logging](#monitoring--logging)
18. [Troubleshooting](#troubleshooting)
19. [Future Roadmap](#future-roadmap)
20. [Contributing](#contributing)

---

## Executive Summary

### What is SmartGarden?

SmartGarden is an **intelligent IoT plant monitoring and automated watering system** that combines hardware sensors, cloud computing, and data analytics to help users maintain optimal plant health. The system monitors environmental conditions in real-time and automatically waters plants based on configurable thresholds.

### Key Features

- **Real-Time Monitoring** - Live sensor data via WebSocket (SignalR)
- **Automated Watering** - Intelligent watering based on soil moisture thresholds
- **Historical Analytics** - Time-series charts with hourly/daily/weekly aggregation
- **Smart Plant Database** - 40,000+ plant species with auto-suggested care thresholds
- **Multi-Device Support** - Manage multiple ESP32 IoT devices
- **Responsive Web Interface** - Modern React 18 application with real-time updates
- **RESTful API** - Comprehensive API with Swagger documentation
- **Advanced Security** - JWT authentication, rate limiting, CORS protection

### Target Users

- **Home Users** - Monitor houseplants, automate watering during travel
- **Gardening Enthusiasts** - Track multiple plants, analyze growth patterns
- **Small Greenhouses** - Scale to 50-100 plants with bulk management
- **Educational Institutions** - Teach IoT, data science, plant biology

### Business Value

- **Reduces Plant Mortality** - Automated care based on real-time data
- **Saves Time** - No manual monitoring required
- **Data-Driven Insights** - Historical trends reveal optimal care patterns
- **Scalable** - From single plant to commercial greenhouse
- **Cost-Effective** - Uses affordable ESP32 hardware

---

## System Overview

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         SMARTGARDEN ECOSYSTEM                        │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────┐          ┌──────────────┐          ┌──────────────┐
│   ESP32     │          │              │          │              │
│   Devices   │◄────────►│   Backend    │◄────────►│   Web App    │
│   (IoT)     │   HTTP   │   .NET 10    │ WebSocket│   React 18   │
└─────────────┘          │    Web API   │          └──────────────┘
      │                  └──────┬───────┘                 │
      │                         │                         │
      │                  ┌──────▼───────┐                │
      │                  │              │                │
      └─────────────────►│  SQL Server  │◄───────────────┘
       Sensor Data       │   Database   │    Query Data
                         └──────────────┘

External APIs:
  - Perenual Plant API (species data)
  - Future: Weather API, Notification services
```

### Data Flow

#### 1. Telemetry Flow
```
ESP32 → POST /api/telemetry → TelemetryController
  ↓
Save to SensorReadings table
  ↓
Check PlantThresholds
  ↓
Decision: WATER or SLEEP?
  ↓
If WATER:
  - Create WateringLog
  - Update Plant.LastWateredDate (ExecuteUpdateAsync)
  - Broadcast SignalR event (isWatering: true)
  ↓
Return command to ESP32
```

#### 2. Real-Time Update Flow
```
Backend receives telemetry
  ↓
SignalR Hub broadcasts "ReceiveUpdate"
  ↓
All connected clients receive update
  ↓
Frontend updates specific plant card
  ↓
UI re-renders with new sensor data
```

#### 3. Analytics Flow
```
User selects date range (7 days)
  ↓
Frontend: GET /api/analytics/plant/{id}/historical
  ↓
Backend queries SensorReadings
  ↓
Aggregate by hourly/daily/weekly
  ↓
Calculate min/max/avg
  ↓
Return time-series data points
  ↓
Frontend renders charts with Recharts
```

### Component Interaction

| Component | Responsibility | Communication |
|-----------|---------------|---------------|
| **ESP32 Device** | Read sensors, execute watering commands | HTTP POST to backend |
| **Backend API** | Business logic, data persistence, real-time broadcasting | RESTful + SignalR |
| **Database** | Store sensor data, plant profiles, user accounts | EF Core ORM |
| **Frontend** | User interface, data visualization, real-time updates | HTTP + WebSocket |
| **SignalR Hub** | Real-time bidirectional communication | WebSocket protocol |

---

## Architecture

### Architectural Pattern

**SmartGarden follows a layered architecture:**

```
┌─────────────────────────────────────────────┐
│         Presentation Layer                   │
│  (React Components, Charts, Forms)          │
└──────────────────┬──────────────────────────┘
                   │ HTTP/WebSocket
┌──────────────────▼──────────────────────────┐
│         API Layer (Controllers)              │
│  (AuthController, PlantController, etc.)    │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│         Business Logic Layer                 │
│  (Services: AuthService, PlantService)      │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│         Data Access Layer                    │
│  (EF Core DbContext, Repositories)          │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│         Database Layer                       │
│  (SQL Server, 18 Tables)                    │
└─────────────────────────────────────────────┘
```

### Design Patterns Used

1. **Repository Pattern** - Data access abstraction via EF Core
2. **Dependency Injection** - Built-in ASP.NET Core DI container
3. **DTO Pattern** - Separate domain models from API contracts
4. **Service Layer Pattern** - Business logic encapsulation
5. **Observer Pattern** - SignalR pub/sub for real-time updates
6. **Strategy Pattern** - Aggregation strategies (hourly/daily/weekly)
7. **Factory Pattern** - JWT token generation
8. **Singleton Pattern** - SignalR hub connections

### SOLID Principles

- **Single Responsibility** - Each controller handles one resource
- **Open/Closed** - Extensible via interfaces (IPlantService, ISensorService)
- **Liskov Substitution** - Service implementations are interchangeable
- **Interface Segregation** - Focused interfaces for each service
- **Dependency Inversion** - Depend on abstractions, not implementations

---

## Technology Stack

### Backend (.NET 10)

| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core** | 10.0 | Web API framework |
| **C#** | 14 | Programming language (with `field` keyword) |
| **Entity Framework Core** | 10.0 | ORM for database operations |
| **SignalR** | Built-in | Real-time WebSocket communication |
| **JWT Bearer** | 10.0 | Authentication middleware |
| **Swagger/OpenAPI** | 6.x | API documentation |
| **AspNetCoreRateLimit** | 5.x | Rate limiting middleware |
| **BCrypt.NET** | 0.1.0 | Password hashing |
| **SQL Server** | 2019+ | Relational database |

### Frontend (React 18)

| Technology | Version | Purpose |
|------------|---------|---------|
| **React** | 18.2 | UI framework |
| **Vite** | 5.x | Build tool and dev server |
| **TailwindCSS** | 3.x | Utility-first CSS framework |
| **Recharts** | 2.x | Data visualization library |
| **Lucide React** | Latest | Icon library |
| **Axios** | 1.x | HTTP client |
| **@microsoft/signalr** | 8.x | SignalR client |

### IoT Hardware

| Component | Model | Purpose |
|-----------|-------|---------|
| **Microcontroller** | ESP32 DevKit | Wi-Fi enabled processor |
| **Soil Sensor** | Capacitive | Moisture measurement |
| **Temperature/Humidity** | DHT22 | Air conditions |
| **Light Sensor** | BH1750/LDR | Ambient light |
| **Water Level** | Ultrasonic/Float | Tank level |
| **Water Pump** | 5V/12V DC | Plant watering |
| **Relay Module** | 5V | Pump control |

### Development Tools

- **Visual Studio 2022 / VS Code** - IDEs
- **Git** - Version control
- **Postman** - API testing
- **SQL Server Management Studio** - Database management
- **Docker** - Containerization (optional)
- **Azure DevOps / GitHub Actions** - CI/CD

---

## Database Design

### Entity Relationship Diagram

```
Users (1) ─────── (*) Plants
  │                   │
  │                   ├── (1) Species
  │                   ├── (1) SoilType
  │                   ├── (*) SensorReadings
  │                   ├── (*) WateringLogs
  │                   ├── (*) PlantThresholds
  │                   ├── (*) PlantPhotos
  │                   └── (*) PlantHealth
  │
  └─── (*) Devices
          │
          └── (*) DeviceCommands
```

### Core Tables

#### 1. Users
```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    -- Shadow Properties (managed by EF Core)
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0
);
```

#### 2. Plants
```sql
CREATE TABLE Plants (
    PlantId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    SpeciesId INT NOT NULL FOREIGN KEY REFERENCES Species(SpeciesId),
    SoilTypeId INT NOT NULL FOREIGN KEY REFERENCES SoilTypes(SoilTypeId),
    Nickname NVARCHAR(200),
    RoomName NVARCHAR(200),
    IsOutdoor BIT DEFAULT 0,
    DateAcquired DATETIME2,
    LastWateredDate DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0
);
```

#### 3. SensorReadings (Time-Series Data)
```sql
CREATE TABLE SensorReadings (
    ReadingId BIGINT PRIMARY KEY IDENTITY(1,1),
    PlantId INT NOT NULL FOREIGN KEY REFERENCES Plants(PlantId),
    SoilMoisture FLOAT NOT NULL,
    AirTemp FLOAT NOT NULL,
    AirHumidity FLOAT NOT NULL,
    LightLevel FLOAT NOT NULL,
    AirQuality FLOAT NOT NULL,
    WaterLevel FLOAT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0,
    INDEX IX_SensorReadings_PlantId_CreatedAt (PlantId, CreatedAt)
);
```

#### 4. PlantThresholds
```sql
CREATE TABLE PlantThresholds (
    PlantThresholdId INT PRIMARY KEY IDENTITY(1,1),
    PlantId INT NOT NULL FOREIGN KEY REFERENCES Plants(PlantId),
    MinSoilMoisture FLOAT,
    MaxSoilMoisture FLOAT,
    MinAirTemp FLOAT,
    MaxAirTemp FLOAT,
    MinAirHumidity FLOAT,
    MaxAirHumidity FLOAT,
    MinLightLevel FLOAT,
    MaxLightLevel FLOAT,
    IdealWateringIntervalHours INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0
);
```

#### 5. Devices
```sql
CREATE TABLE Devices (
    DeviceId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    PlantId INT FOREIGN KEY REFERENCES Plants(PlantId),
    DeviceName NVARCHAR(200) NOT NULL,
    DeviceToken NVARCHAR(MAX) NOT NULL,
    MacAddress NVARCHAR(50),
    IpAddress NVARCHAR(50),
    FirmwareVersion NVARCHAR(50),
    Model NVARCHAR(100),
    SerialNumber NVARCHAR(100),
    IsOnline BIT DEFAULT 0,
    LastSeen DATETIME2,
    LastHeartbeat DATETIME2,
    BatteryLevel INT,
    SignalStrength INT,
    ReadingIntervalSec INT DEFAULT 300,
    IsCalibrated BIT DEFAULT 0,
    CalibrationDate DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0
);
```

#### 6. WateringLogs
```sql
CREATE TABLE WateringLogs (
    WateringId BIGINT PRIMARY KEY IDENTITY(1,1),
    PlantId INT NOT NULL FOREIGN KEY REFERENCES Plants(PlantId),
    DurationSec INT NOT NULL,
    Mode INT NOT NULL, -- 0=Manual, 1=Auto, 2=Scheduled
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0,
    INDEX IX_WateringLogs_PlantId_CreatedAt (PlantId, CreatedAt)
);
```

### Indexing Strategy

**Performance-Critical Indexes:**
```sql
-- Sensor readings queries (most frequent)
CREATE INDEX IX_SensorReadings_PlantId_CreatedAt
ON SensorReadings(PlantId, CreatedAt DESC);

-- Watering logs timeline
CREATE INDEX IX_WateringLogs_PlantId_CreatedAt
ON WateringLogs(PlantId, CreatedAt DESC);

-- Device lookup by MAC address
CREATE INDEX IX_Devices_MacAddress
ON Devices(MacAddress);

-- User email lookup (login)
CREATE INDEX IX_Users_Email
ON Users(Email);
```

### Data Retention Policy

```sql
-- Auto-purge sensor data older than 90 days
-- Executed by background service
DELETE FROM SensorReadings
WHERE CreatedAt < DATEADD(day, -90, GETUTCDATE());

-- Archive watering logs older than 1 year
-- Move to WateringLogsArchive table
```

---

## Backend API

### Project Structure

```
SmartGarden.API/
├── Controllers/
│   ├── AuthController.cs           # User authentication
│   ├── PlantController.cs          # Plant CRUD + smart search
│   ├── TelemetryController.cs      # ESP32 telemetry processing
│   ├── AnalyticsController.cs      # Historical data aggregation
│   ├── DeviceController.cs         # Device management
│   ├── SensorController.cs         # Sensor data retrieval
│   ├── WateringController.cs       # Watering control
│   └── AlertController.cs          # Alerts and notifications
├── Hubs/
│   └── PlantHub.cs                 # SignalR real-time hub
├── Services/
│   ├── AuthService.cs              # Authentication logic
│   ├── PlantService.cs             # Plant business logic
│   ├── PlantInfoService.cs         # Perenual API integration
│   ├── DeviceService.cs            # Device management logic
│   ├── SensorService.cs            # Sensor data processing
│   ├── WateringService.cs          # Watering logic
│   └── AutoWateringBackgroundService.cs  # Background tasks
├── Middleware/
│   └── (Future: Custom middleware)
├── Program.cs                      # App configuration
├── appsettings.json               # Configuration
└── appsettings.Development.json   # Dev config

SmartGarden.Core/
├── Models/                         # Domain entities
├── DTOs/                          # Data transfer objects
├── Interfaces/                    # Service contracts
└── Shared/                        # Enums, constants

SmartGarden.Data/
├── Persistence/
│   └── SmartGardenDbContext.cs    # EF Core context
├── Migrations/                    # Database migrations
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

### API Endpoints

#### Authentication Endpoints
```http
POST   /api/auth/register          # User registration
POST   /api/auth/login             # User login
GET    /api/auth/profile           # Get current user
```

#### Plant Endpoints
```http
GET    /api/plant                  # Get all user's plants
GET    /api/plant/{id}             # Get plant by ID
POST   /api/plant                  # Create new plant
PUT    /api/plant/{id}             # Update plant
DELETE /api/plant/{id}             # Delete plant (soft)
GET    /api/plant/search?q={query} # Smart plant search
POST   /api/plant/from-search      # Create from search result
```

#### Telemetry Endpoints
```http
POST   /api/telemetry              # Receive ESP32 sensor data
GET    /api/telemetry/health       # Health check
```

#### Analytics Endpoints
```http
GET    /api/analytics/plant/{id}/historical
       ?startDate={date}&endDate={date}&interval={hourly|daily|weekly}
GET    /api/analytics/plant/{id}/summary?days={7}
```

#### Device Endpoints
```http
GET    /api/device                 # Get all devices
GET    /api/device/{id}            # Get device by ID
POST   /api/device                 # Register new device
PUT    /api/device/{id}            # Update device
DELETE /api/device/{id}            # Delete device
POST   /api/device/{id}/approve    # Approve pending device
```

#### Sensor Endpoints
```http
GET    /api/sensor/{plantId}/latest      # Latest reading
GET    /api/sensor/{plantId}/history     # Historical readings
POST   /api/sensor/reading               # Manual reading
```

#### Watering Endpoints
```http
POST   /api/watering/{plantId}/manual    # Manual watering
PUT    /api/watering/{plantId}/auto      # Configure auto-watering
GET    /api/watering/{plantId}/logs      # Watering history
```

### Request/Response Examples

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "userId": 1,
    "username": "john_doe",
    "email": "john@example.com"
  },
  "expiresAt": "2025-11-26T10:30:00Z"
}
```

#### Send Telemetry (ESP32)
```http
POST /api/telemetry
Content-Type: application/json
Authorization: Bearer {device-token}

{
  "deviceId": 1,
  "soilMoisture": 35.5,
  "tankLevel": 85.0,
  "airTemp": 22.3,
  "airHumidity": 65.2,
  "lightLevel": 1200.5,
  "airQuality": 50.0
}
```

**Response (Need Watering):**
```json
{
  "command": "WATER",
  "duration": 5,
  "message": "Soil moisture low (35.5% < 40.0%)",
  "nextCheckInSeconds": 300
}
```

**Response (No Watering):**
```json
{
  "command": "SLEEP",
  "message": "Soil moisture adequate (45.2% >= 40.0%)",
  "nextCheckInSeconds": 300
}
```

#### Get Historical Analytics
```http
GET /api/analytics/plant/1/historical
    ?startDate=2025-11-17T00:00:00Z
    &endDate=2025-11-24T23:59:59Z
    &interval=daily
Authorization: Bearer {user-token}
```

**Response:**
```json
{
  "plantId": 1,
  "plantName": "My Basil",
  "startDate": "2025-11-17T00:00:00Z",
  "endDate": "2025-11-24T23:59:59Z",
  "interval": "daily",
  "dataPoints": [
    {
      "timestamp": "2025-11-17T00:00:00Z",
      "avgSoilMoisture": 45.2,
      "minSoilMoisture": 30.1,
      "maxSoilMoisture": 62.5,
      "avgAirTemp": 22.5,
      "avgLightLevel": 1200.5,
      "readingCount": 24
    }
    // ... more data points
  ],
  "wateringEvents": [
    {
      "wateringId": 123,
      "timestamp": "2025-11-17T08:30:00Z",
      "durationSec": 5,
      "mode": "Auto"
    }
  ],
  "statistics": {
    "avgSoilMoisture": 45.2,
    "minSoilMoisture": 25.0,
    "maxSoilMoisture": 65.0,
    "totalReadings": 168,
    "totalWaterings": 4
  }
}
```

### Error Handling

**Standard Error Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."],
    "Password": ["Password must be at least 8 characters."]
  }
}
```

**Custom Error Response:**
```json
{
  "error": "Plant not found",
  "plantId": 999,
  "timestamp": "2025-11-25T10:30:00Z"
}
```

### HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET, PUT, POST (no creation) |
| 201 | Created | Successful POST with resource creation |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Invalid input data |
| 401 | Unauthorized | Missing or invalid JWT token |
| 403 | Forbidden | Valid token but insufficient permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate resource (e.g., email exists) |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server exception |

---

## Frontend Application

### Component Hierarchy

```
App.jsx (Root)
│
├── LoginScreen
│   └── Form inputs, validation
│
├── SignUpScreen
│   └── Registration form
│
├── CalibrationScreen
│   └── CalibrationModal
│       └── Sensor calibration wizard
│
├── DashboardScreen
│   ├── PlantCard (multiple)
│   │   └── Real-time sensor data
│   ├── AddPlantWizard
│   │   └── Smart plant search
│   └── Menu sidebar
│
└── PlantDetailScreen
    ├── Sensor readings display
    ├── Manual watering button
    ├── Auto-watering config
    ├── Weekly statistics chart
    └── PlantAnalytics (modal/page)
        ├── Date range selector
        ├── Metric toggles
        ├── Line charts (Recharts)
        └── Statistics cards
```

### State Management

**Global State (useState hooks):**
```javascript
const [currentScreen, setCurrentScreen] = useState('login');
const [user, setUser] = useState(null);
const [plants, setPlants] = useState([]);
const [selectedPlant, setSelectedPlant] = useState(null);
```

**SignalR Connection (useRef hook):**
```javascript
const connectionRef = useRef(null);

useEffect(() => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${ENV.API.BASE_URL}/hubs/plant`)
    .withAutomaticReconnect()
    .build();

  connection.on('ReceiveUpdate', (update) => {
    setPlants(prevPlants => /* update specific plant */);
  });

  connectionRef.current = connection;
}, []);
```

### Styling Approach

**TailwindCSS Utility Classes:**
```jsx
<div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-2xl transition-all">
  <h2 className="text-2xl font-bold text-gray-800 mb-4">
    Plant Dashboard
  </h2>
</div>
```

**Responsive Design:**
```jsx
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  {/* Adapts: 1 column mobile, 2 tablet, 3 desktop */}
</div>
```

### API Integration

**Service Pattern:**
```javascript
// src/api/plantService.js
const plantService = {
  async getAllPlants() {
    const response = await apiClient.get(`${ENV.API.BASE_URL}/plant`);
    return response;
  },

  async searchPlants(query) {
    const response = await apiClient.get(
      `${ENV.API.BASE_URL}/plant/search?q=${encodeURIComponent(query)}`
    );
    return response;
  }
};
```

**Usage in Component:**
```javascript
import { plantService } from '../api';

const fetchPlants = async () => {
  try {
    const plants = await plantService.getAllPlants();
    setPlants(plants);
  } catch (error) {
    setError(error.message);
  }
};
```

### Real-Time Updates

**SignalR Integration:**
```javascript
// Connect to hub
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${ENV.API.BASE_URL}/hubs/plant`, {
    transport: signalR.HttpTransportType.WebSockets,
    withCredentials: true
  })
  .withAutomaticReconnect()
  .build();

// Listen for updates
connection.on('ReceiveUpdate', (update) => {
  console.log('Real-time update:', update);
  // Update UI instantly
});

// Start connection
await connection.start();
```

### Chart Visualization

**Recharts Configuration:**
```jsx
<ResponsiveContainer width="100%" height={400}>
  <LineChart data={chartData}>
    <CartesianGrid strokeDasharray="3 3" />
    <XAxis dataKey="displayTime" />
    <YAxis />
    <Tooltip content={<CustomTooltip />} />
    <Legend />
    <Line
      type="monotone"
      dataKey="soilMoisture"
      stroke="#22c55e"
      strokeWidth={2}
    />
  </LineChart>
</ResponsiveContainer>
```

---

## Security

### Authentication Flow

```
1. User sends credentials → POST /api/auth/login
2. Backend validates credentials (BCrypt hash comparison)
3. Backend generates JWT token with claims:
   - userId
   - email
   - type: "user"
   - exp: expiration timestamp
4. Backend returns token to client
5. Client stores token (localStorage/sessionStorage)
6. Client includes token in subsequent requests:
   Authorization: Bearer {token}
7. Backend validates token on each request
8. If valid: process request
   If invalid/expired: return 401 Unauthorized
```

### JWT Token Structure

**Claims:**
```json
{
  "userId": "123",
  "email": "user@example.com",
  "type": "user",
  "nbf": 1700000000,
  "exp": 1700086400,
  "iat": 1700000000,
  "iss": "SmartGarden",
  "aud": "SmartGarden"
}
```

### Dual Authentication Schemes

**User Authentication:**
```csharp
services.AddAuthentication("UserAuth")
  .AddJwtBearer("UserAuth", options => {
    options.TokenValidationParameters = new TokenValidationParameters {
      ValidateIssuer = true,
      ValidIssuer = "SmartGarden",
      IssuerSigningKey = new SymmetricSecurityKey(userSecret)
    };
  });
```

**Device Authentication:**
```csharp
services.AddAuthentication("DeviceAuth")
  .AddJwtBearer("DeviceAuth", options => {
    options.TokenValidationParameters = new TokenValidationParameters {
      ClockSkew = TimeSpan.FromMinutes(5), // Tolerance for ESP32 time drift
      IssuerSigningKey = new SymmetricSecurityKey(deviceSecret)
    };
  });
```

### Authorization Policies

```csharp
[Authorize(Policy = "UserOnly")]
public async Task<IActionResult> GetUserProfile() { }

[Authorize(Policy = "DeviceOnly")]
public async Task<IActionResult> SubmitTelemetry() { }

[Authorize(Policy = "UserOrDevice")]
public async Task<IActionResult> GetPlantData() { }
```

### Password Security

**BCrypt Hashing:**
```csharp
// Registration
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

// Login verification
bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
```

### Rate Limiting

**Configuration:**
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1s",
        "Limit": 10
      },
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "1m",
        "Limit": 5
      },
      {
        "Endpoint": "POST:/api/telemetry",
        "Period": "5s",
        "Limit": 1
      }
    ]
  }
}
```

### CORS Policy

```csharp
services.AddCors(options => {
  options.AddPolicy("SmartGardenCorsPolicy", policy => {
    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials(); // Required for SignalR
  });
});
```

### Input Validation

**Data Annotations:**
```csharp
public class TelemetryRequestDto
{
    [Required]
    public int DeviceId { get; set; }

    [Required]
    [Range(0, 100)]
    public double SoilMoisture { get; set; }

    [Required]
    [Range(0, 100)]
    public double TankLevel { get; set; }
}
```

### SQL Injection Prevention

**EF Core Parameterized Queries (automatic):**
```csharp
// Safe - parameterized by EF Core
var plants = await _context.Plants
    .Where(p => p.UserId == userId)
    .ToListAsync();

// Unsafe - never use raw SQL with string interpolation
// var plants = _context.Plants.FromSqlRaw($"SELECT * FROM Plants WHERE UserId = {userId}");
```

### Sensitive Data Protection

**appsettings.json (Development):**
```json
{
  "JwtSettings": {
    "UserSecret": "dev-secret-key-32-chars-minimum",
    "DeviceSecret": "dev-device-secret-32-chars-min"
  }
}
```

**Environment Variables (Production):**
```bash
export JWT_USER_SECRET="prod-secret-from-key-vault"
export JWT_DEVICE_SECRET="prod-device-secret-from-key-vault"
```

**Configuration:**
```csharp
var userSecret = builder.Configuration["JwtSettings:UserSecret"]
    ?? Environment.GetEnvironmentVariable("JWT_USER_SECRET");
```

---

## Real-Time Communication

### SignalR Hub Implementation

**PlantHub.cs:**
```csharp
public class PlantHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var userId = Context.User?.FindFirst("userId")?.Value;
        _logger.LogInformation("Client connected: {ConnectionId}", connectionId);
        await base.OnConnectedAsync();
    }

    public async Task SubscribeToPlant(int plantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Plant_{plantId}");
    }
}
```

### Broadcasting Updates

**From TelemetryController:**
```csharp
private async Task BroadcastPlantUpdateAsync(int plantId, TelemetryRequestDto request, string plantName, bool isWatering)
{
    var update = new PlantUpdateDto
    {
        PlantId = plantId,
        SoilMoisture = request.SoilMoisture,
        WaterLevel = request.TankLevel,
        IsWatering = isWatering,
        Timestamp = DateTime.UtcNow
    };

    // Broadcast to all connected clients
    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", update);

    // OR broadcast to specific plant group
    // await _hubContext.Clients.Group($"Plant_{plantId}").SendAsync("ReceiveUpdate", update);
}
```

### Frontend SignalR Client

**Connection Setup:**
```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5000/hubs/plant', {
    skipNegotiation: false,
    transport: signalR.HttpTransportType.WebSockets |
               signalR.HttpTransportType.ServerSentEvents,
    withCredentials: true
  })
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: (retryContext) => {
      return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
    }
  })
  .configureLogging(signalR.LogLevel.Information)
  .build();
```

**Event Handlers:**
```javascript
// Receive updates
connection.on('ReceiveUpdate', (update) => {
  setPlants(prevPlants =>
    prevPlants.map(plant =>
      plant.plantId === update.plantId
        ? { ...plant, ...update }
        : plant
    )
  );
});

// Reconnection events
connection.onreconnecting((error) => {
  console.warn('Reconnecting...', error);
  setConnectionStatus('Reconnecting');
});

connection.onreconnected(() => {
  console.log('Reconnected successfully');
  setConnectionStatus('Connected');
});

connection.onclose((error) => {
  console.error('Connection closed', error);
  setConnectionStatus('Disconnected');
});
```

### Performance Considerations

**Message Size Optimization:**
- Only send changed data, not full objects
- Use compact field names
- Compress large payloads

**Connection Scaling:**
- Use Azure SignalR Service for 10,000+ concurrent connections
- Implement backplane (Redis) for multi-server deployments

---

## IoT Integration

### ESP32 Firmware Architecture

**Main Loop:**
```cpp
void setup() {
  Serial.begin(115200);
  connectWiFi();
  authenticateDevice();
  initSensors();
}

void loop() {
  // 1. Read sensors
  SensorData data = readAllSensors();

  // 2. Send to backend
  TelemetryResponse response = sendTelemetry(data);

  // 3. Execute command
  if (response.command == "WATER") {
    waterPlant(response.duration);
  }

  // 4. Sleep until next reading
  delay(response.nextCheckInSeconds * 1000);
}
```

### Sensor Reading Functions

**Soil Moisture:**
```cpp
float readSoilMoisture() {
  int rawValue = analogRead(SOIL_MOISTURE_PIN);
  // Calibration: map raw value to percentage
  float moisture = map(rawValue, dryCalibration, wetCalibration, 0, 100);
  return constrain(moisture, 0, 100);
}
```

**Temperature & Humidity (DHT22):**
```cpp
#include <DHT.h>
DHT dht(DHT_PIN, DHT22);

void readDHT22(float &temp, float &humidity) {
  humidity = dht.readHumidity();
  temp = dht.readTemperature(); // Celsius

  if (isnan(humidity) || isnan(temp)) {
    Serial.println("Failed to read from DHT sensor!");
  }
}
```

**Light Level (BH1750):**
```cpp
#include <BH1750.h>
BH1750 lightMeter;

float readLightLevel() {
  float lux = lightMeter.readLightLevel();
  return lux;
}
```

### HTTP Communication

**Send Telemetry:**
```cpp
#include <HTTPClient.h>
#include <ArduinoJson.h>

TelemetryResponse sendTelemetry(SensorData data) {
  HTTPClient http;
  http.begin(API_URL "/telemetry");
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + deviceToken);

  // Build JSON payload
  StaticJsonDocument<512> doc;
  doc["deviceId"] = DEVICE_ID;
  doc["soilMoisture"] = data.soilMoisture;
  doc["tankLevel"] = data.waterLevel;
  doc["airTemp"] = data.airTemp;
  doc["airHumidity"] = data.airHumidity;
  doc["lightLevel"] = data.lightLevel;

  String payload;
  serializeJson(doc, payload);

  // Send request
  int httpCode = http.POST(payload);
  String response = http.getString();
  http.end();

  // Parse response
  DynamicJsonDocument responseDoc(512);
  deserializeJson(responseDoc, response);

  TelemetryResponse result;
  result.command = responseDoc["command"].as<String>();
  result.duration = responseDoc["duration"] | 0;
  result.nextCheckInSeconds = responseDoc["nextCheckInSeconds"] | 300;

  return result;
}
```

### Watering Control

**Pump Activation:**
```cpp
#define RELAY_PIN 5

void waterPlant(int durationSec) {
  Serial.printf("Watering for %d seconds...\n", durationSec);

  digitalWrite(RELAY_PIN, HIGH);  // Turn on pump
  delay(durationSec * 1000);
  digitalWrite(RELAY_PIN, LOW);   // Turn off pump

  Serial.println("Watering complete.");
}
```

### Power Management

**Deep Sleep (Battery Optimization):**
```cpp
#include <esp_sleep.h>

void enterDeepSleep(int seconds) {
  Serial.printf("Entering deep sleep for %d seconds\n", seconds);
  esp_sleep_enable_timer_wakeup(seconds * 1000000ULL);
  esp_deep_sleep_start();
}

// Use instead of delay() for battery-powered devices
void loop() {
  SensorData data = readAllSensors();
  sendTelemetry(data);

  // Sleep for 5 minutes (instead of delay)
  enterDeepSleep(300);
}
```

### Error Handling

**WiFi Reconnection:**
```cpp
void connectWiFi() {
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  int retries = 0;
  while (WiFi.status() != WL_CONNECTED && retries < 20) {
    delay(500);
    Serial.print(".");
    retries++;
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("\nWiFi connected");
    Serial.println(WiFi.localIP());
  } else {
    Serial.println("\nFailed to connect to WiFi");
    // Retry after delay or restart
    ESP.restart();
  }
}
```

---

## Installation & Setup

### Prerequisites

**Software Requirements:**
- .NET 10 SDK
- Node.js 18+
- SQL Server 2019+ or Docker
- Git
- Visual Studio 2022 / VS Code (optional)

**Hardware Requirements (Optional):**
- ESP32 DevKit
- Sensors (DHT22, soil moisture, etc.)
- Water pump and relay module

### Step-by-Step Installation

#### 1. Clone Repository
```bash
git clone https://github.com/your-org/SmartGarden.git
cd SmartGarden
```

#### 2. Database Setup

**Option A: Windows SQL Server**
```bash
# Install SQL Server Express from:
# https://www.microsoft.com/en-us/sql-server/sql-server-downloads

# Create database
sqlcmd -S localhost\SQLEXPRESS -Q "CREATE DATABASE SmartGardenDB"
```

**Option B: Docker (Mac/Linux)**
```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 \
  --name smartgarden-sql \
  -d mcr.microsoft.com/mssql/server:2019-latest
```

#### 3. Backend Configuration

**Edit `appsettings.json`:**
```bash
cd MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API
```

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SmartGardenDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "UserSecret": "your-32-char-secret-key-change-in-production",
    "DeviceSecret": "your-32-char-device-secret-change-in-prod",
    "Issuer": "SmartGarden",
    "Audience": "SmartGarden"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

#### 4. Run Database Migrations
```bash
dotnet restore
dotnet ef database update
```

#### 5. Start Backend
```bash
dotnet run
# Backend running at: http://localhost:5000
```

#### 6. Frontend Setup
```bash
cd ReactNativeApp/SmartGardenApp
npm install
npm install recharts
```

**Edit `src/config/env.js`:**
```javascript
const ENV = {
  API: {
    BASE_URL: 'http://localhost:5000/api'
  }
};
```

#### 7. Start Frontend
```bash
npm run dev
# Frontend running at: http://localhost:5173
```

#### 8. Verify Installation

**Test Backend:**
```bash
curl http://localhost:5000/api/telemetry/health
```

**Test Frontend:**
- Open browser: http://localhost:5173
- Create account
- Login successful = ✅ Installation complete!

---

## Configuration

### Backend Configuration Files

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SmartGardenDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "UserSecret": "minimum-32-characters-secret-key-for-production",
    "DeviceSecret": "minimum-32-characters-device-secret-for-production",
    "Issuer": "SmartGarden",
    "Audience": "SmartGarden",
    "ExpirationMinutes": 1440
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000"
    ]
  },
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1s",
        "Limit": 10
      }
    ]
  }
}
```

**appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "*** Use Azure Key Vault ***"
  },
  "JwtSettings": {
    "UserSecret": "*** Use Environment Variable ***",
    "DeviceSecret": "*** Use Environment Variable ***"
  }
}
```

### Environment Variables

**Development (.env file):**
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000
```

**Production (Azure/AWS):**
```bash
ASPNETCORE_ENVIRONMENT=Production
JWT_USER_SECRET=prod-secret-from-key-vault
JWT_DEVICE_SECRET=prod-device-secret-from-key-vault
DATABASE_CONNECTION_STRING=prod-sql-connection-string
```

### Frontend Configuration

**src/config/env.js:**
```javascript
const ENV = {
  API: {
    BASE_URL: process.env.NODE_ENV === 'production'
      ? 'https://api.smartgarden.com/api'
      : 'http://localhost:5000/api',
    TIMEOUT: 30000
  },
  SENSORS: {
    CALIBRATION_COUNTDOWN: 5,
    POLLING_INTERVAL: 5000
  },
  SIGNALR: {
    HUB_URL: process.env.NODE_ENV === 'production'
      ? 'https://api.smartgarden.com/hubs/plant'
      : 'http://localhost:5000/hubs/plant'
  }
};

export default ENV;
```

---

## Development Guide

### Setting Up Development Environment

**Recommended Tools:**
- Visual Studio 2022 (Windows) or Rider (Mac/Linux)
- VS Code with C# extension
- SQL Server Management Studio (SSMS)
- Postman for API testing
- React DevTools browser extension

### Backend Development Workflow

**1. Create New Controller:**
```bash
cd SmartGarden.API/Controllers
# Create NotificationController.cs
```

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
    {
        var notifications = await _notificationService.GetUserNotificationsAsync();
        return Ok(notifications);
    }
}
```

**2. Register Service:**
```csharp
// Program.cs
builder.Services.AddScoped<INotificationService, NotificationService>();
```

**3. Create Migration:**
```bash
dotnet ef migrations add AddNotificationTable
dotnet ef database update
```

**4. Test Endpoint:**
```bash
# Using Postman or curl
curl -X GET http://localhost:5000/api/notification \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Frontend Development Workflow

**1. Create New Component:**
```bash
cd src/components
# Create NotificationBell.jsx
```

```jsx
import React, { useState, useEffect } from 'react';
import { Bell } from 'lucide-react';
import { notificationService } from '../api';

const NotificationBell = () => {
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    fetchNotifications();
  }, []);

  const fetchNotifications = async () => {
    const data = await notificationService.getNotifications();
    setNotifications(data);
    setUnreadCount(data.filter(n => !n.isRead).length);
  };

  return (
    <div className="relative">
      <Bell className="w-6 h-6 cursor-pointer" />
      {unreadCount > 0 && (
        <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
          {unreadCount}
        </span>
      )}
    </div>
  );
};

export default NotificationBell;
```

**2. Create API Service:**
```javascript
// src/api/notificationService.js
const notificationService = {
  async getNotifications() {
    const response = await apiClient.get(`${ENV.API.BASE_URL}/notification`);
    return response;
  }
};

export default notificationService;
```

**3. Import and Use:**
```jsx
import NotificationBell from './components/NotificationBell';

function DashboardScreen() {
  return (
    <div>
      <NotificationBell />
      {/* Other components */}
    </div>
  );
}
```

### Code Style Guidelines

**Backend (C#):**
- Use PascalCase for public members
- Use camelCase for private fields with `_` prefix
- Async methods end with `Async` suffix
- Use `var` for obvious types
- XML documentation for public APIs

```csharp
/// <summary>
/// Gets a plant by ID
/// </summary>
/// <param name="plantId">The plant identifier</param>
/// <returns>Plant entity or null</returns>
public async Task<Plant?> GetPlantByIdAsync(int plantId)
{
    return await _context.Plants
        .Include(p => p.Species)
        .FirstOrDefaultAsync(p => p.PlantId == plantId);
}
```

**Frontend (JavaScript/React):**
- Use camelCase for variables and functions
- Use PascalCase for React components
- Use arrow functions for callbacks
- Destructure props
- Use meaningful variable names

```jsx
const PlantCard = ({ plant, onSelect }) => {
  const [isHovered, setIsHovered] = useState(false);

  const handleClick = () => {
    onSelect(plant.plantId);
  };

  return (
    <div
      className="plant-card"
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      onClick={handleClick}
    >
      {/* Card content */}
    </div>
  );
};
```

### Git Workflow

**Branching Strategy:**
```bash
main                    # Production-ready code
  ├── develop          # Integration branch
  │   ├── feature/smart-notifications
  │   ├── feature/weather-integration
  │   └── bugfix/sensor-reading-null
  └── hotfix/critical-security-patch
```

**Commit Message Format:**
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Examples:**
```bash
feat(analytics): Add export to CSV functionality

- Add CSV export button to analytics page
- Implement data formatting for Excel compatibility
- Add download trigger with proper filename

Closes #123
```

```bash
fix(telemetry): Handle null device gracefully

Prevents 500 error when device is not found.
Returns 404 with clear error message instead.

Fixes #456
```

**Commit Types:**
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation only
- `style` - Code style changes (formatting)
- `refactor` - Code refactoring
- `perf` - Performance improvement
- `test` - Adding tests
- `chore` - Build/tooling changes

---

## Testing

### Backend Unit Tests

**Create Test Project:**
```bash
dotnet new xunit -n SmartGarden.Tests
cd SmartGarden.Tests
dotnet add reference ../SmartGarden.API/SmartGarden.API.csproj
dotnet add package Moq
dotnet add package FluentAssertions
```

**Example Test:**
```csharp
using Xunit;
using Moq;
using FluentAssertions;

public class PlantServiceTests
{
    private readonly Mock<SmartGardenDbContext> _mockContext;
    private readonly PlantService _plantService;

    public PlantServiceTests()
    {
        _mockContext = new Mock<SmartGardenDbContext>();
        _plantService = new PlantService(_mockContext.Object);
    }

    [Fact]
    public async Task GetPlantById_ExistingId_ReturnsPlant()
    {
        // Arrange
        int plantId = 1;
        var expectedPlant = new Plant { PlantId = plantId, Nickname = "Test Plant" };
        _mockContext.Setup(c => c.Plants.FindAsync(plantId))
            .ReturnsAsync(expectedPlant);

        // Act
        var result = await _plantService.GetPlantByIdAsync(plantId);

        // Assert
        result.Should().NotBeNull();
        result.Nickname.Should().Be("Test Plant");
    }

    [Fact]
    public async Task GetPlantById_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockContext.Setup(c => c.Plants.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Plant)null);

        // Act
        var result = await _plantService.GetPlantByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
```

### Integration Tests

**Test with In-Memory Database:**
```csharp
using Microsoft.EntityFrameworkCore;

public class TelemetryControllerIntegrationTests : IDisposable
{
    private readonly SmartGardenDbContext _context;
    private readonly TelemetryController _controller;

    public TelemetryControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<SmartGardenDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new SmartGardenDbContext(options);
        _controller = new TelemetryController(_context, Mock.Of<ILogger>(), Mock.Of<IHubContext>());

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.Devices.Add(new Device { DeviceId = 1, DeviceName = "Test Device" });
        _context.Plants.Add(new Plant { PlantId = 1, Nickname = "Test Plant" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task ReceiveTelemetry_ValidData_SavesReading()
    {
        // Arrange
        var request = new TelemetryRequestDto
        {
            DeviceId = 1,
            SoilMoisture = 45.5,
            TankLevel = 85.0
        };

        // Act
        var result = await _controller.ReceiveTelemetry(request);

        // Assert
        var readings = await _context.SensorReadings.ToListAsync();
        readings.Should().HaveCount(1);
        readings[0].SoilMoisture.Should().Be(45.5);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

### Frontend Tests

**Install Testing Libraries:**
```bash
npm install --save-dev @testing-library/react @testing-library/jest-dom vitest
```

**Example Component Test:**
```jsx
import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import PlantCard from './PlantCard';

describe('PlantCard', () => {
  it('renders plant nickname', () => {
    const plant = { plantId: 1, nickname: 'My Basil', soilMoisture: 45.5 };
    render(<PlantCard plant={plant} />);

    expect(screen.getByText('My Basil')).toBeInTheDocument();
  });

  it('calls onSelect when clicked', () => {
    const plant = { plantId: 1, nickname: 'My Basil' };
    const handleSelect = vi.fn();

    render(<PlantCard plant={plant} onSelect={handleSelect} />);
    fireEvent.click(screen.getByText('My Basil'));

    expect(handleSelect).toHaveBeenCalledWith(1);
  });

  it('displays soil moisture percentage', () => {
    const plant = { plantId: 1, soilMoisture: 45.5 };
    render(<PlantCard plant={plant} />);

    expect(screen.getByText('45.5%')).toBeInTheDocument();
  });
});
```

### E2E Tests (Cypress)

```bash
npm install --save-dev cypress
npx cypress open
```

**cypress/e2e/login.cy.js:**
```javascript
describe('User Login', () => {
  it('successfully logs in', () => {
    cy.visit('http://localhost:5173');

    cy.get('input[placeholder="Email"]').type('test@example.com');
    cy.get('input[placeholder="Password"]').type('password123');
    cy.get('button').contains('Login').click();

    cy.url().should('include', '/dashboard');
    cy.contains('My Plants').should('be.visible');
  });

  it('shows error with invalid credentials', () => {
    cy.visit('http://localhost:5173');

    cy.get('input[placeholder="Email"]').type('wrong@example.com');
    cy.get('input[placeholder="Password"]').type('wrongpass');
    cy.get('button').contains('Login').click();

    cy.contains('Login failed').should('be.visible');
  });
});
```

---

## Deployment

### Production Checklist

- [ ] Change JWT secrets to strong production values
- [ ] Update CORS origins to production URLs
- [ ] Enable HTTPS/SSL
- [ ] Configure production database connection string
- [ ] Set up environment variables (Azure Key Vault / AWS Secrets Manager)
- [ ] Enable logging and monitoring
- [ ] Configure CDN for frontend assets
- [ ] Set up database backups
- [ ] Configure rate limiting for production load
- [ ] Review and update security headers
- [ ] Enable application insights/monitoring
- [ ] Configure CI/CD pipeline

### Azure Deployment

**Backend (Azure App Service):**

```bash
# 1. Create Azure resources
az group create --name smartgarden-rg --location eastus
az sql server create --name smartgarden-sql --resource-group smartgarden-rg
az sql db create --name SmartGardenDB --server smartgarden-sql

# 2. Deploy backend
cd SmartGarden.API
dotnet publish -c Release -o ./publish
az webapp create --name smartgarden-api --resource-group smartgarden-rg
az webapp deployment source config-zip --name smartgarden-api --src publish.zip

# 3. Configure environment variables
az webapp config appsettings set --name smartgarden-api \
  --settings JWT_USER_SECRET="prod-secret" \
  ConnectionStrings__DefaultConnection="prod-connection-string"
```

**Frontend (Azure Static Web Apps):**

```bash
# Build production frontend
cd ReactNativeApp/SmartGardenApp
npm run build

# Deploy to Azure Static Web Apps
az staticwebapp create \
  --name smartgarden-web \
  --resource-group smartgarden-rg \
  --source ./dist \
  --location eastus \
  --branch main
```

### Docker Deployment

**Dockerfile (Backend):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SmartGarden.API/SmartGarden.API.csproj", "SmartGarden.API/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/SmartGarden.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartGarden.API.dll"]
```

**docker-compose.yml:**
```yaml
version: '3.8'

services:
  backend:
    build:
      context: ./MobileApp/SmartGarden/SmartGarden.Project
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=SmartGardenDB;User=sa;Password=YourStrong@Passw0rd
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

  frontend:
    build:
      context: ./ReactNativeApp/SmartGardenApp
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    environment:
      - REACT_APP_API_URL=http://backend/api

volumes:
  sqldata:
```

**Deploy with Docker:**
```bash
docker-compose up -d
```

---

*This documentation is maintained by the SmartGarden development team. Last updated: November 2025.*
