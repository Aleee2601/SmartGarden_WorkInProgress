# SmartGarden Backend API 🌱

## Overview

The SmartGarden Backend API is a modern .NET 10 Web API that powers the SmartGarden IoT plant monitoring system. Built with C# 14, Entity Framework Core 10, and SignalR, it provides real-time plant monitoring, automated watering control, and comprehensive analytics.

## Technology Stack

- **.NET 10** - Latest .NET framework
- **C# 14** - Modern C# features including `field` keyword
- **Entity Framework Core 10** - ORM with advanced optimization features
- **SignalR** - Real-time WebSocket communication
- **SQL Server** - Primary database
- **JWT Authentication** - Secure API and device authentication
- **Swagger/OpenAPI** - Interactive API documentation

## Project Structure

```
SmartGarden.API/
├── Controllers/          # API endpoints
│   ├── AuthController.cs           # User authentication
│   ├── PlantController.cs          # Plant management
│   ├── TelemetryController.cs      # ESP32 sensor data
│   ├── AnalyticsController.cs      # Historical analytics
│   ├── DeviceController.cs         # Device registration
│   └── AlertController.cs          # Alert management
├── Hubs/                # SignalR hubs
│   └── PlantHub.cs                 # Real-time plant updates
├── Services/            # Business logic
│   ├── PlantInfoService.cs         # Perenual API integration
│   ├── EmailService.cs             # Email notifications
│   └── ExportService.cs            # Data export
├── Middleware/          # Custom middleware
│   └── ExceptionHandlingMiddleware.cs
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration

SmartGarden.Core/
├── Models/              # Domain entities
│   ├── User.cs
│   ├── Plant.cs
│   ├── SensorReading.cs
│   ├── WateringLog.cs
│   └── Alert.cs
├── DTOs/                # Data transfer objects
│   ├── TelemetryDtos.cs
│   ├── AnalyticsDtos.cs
│   └── PlantSearchDtos.cs
└── Interfaces/          # Service contracts
    └── IPlantInfoService.cs

SmartGarden.Data/
├── SmartGardenContext.cs    # EF Core DbContext
├── Migrations/              # Database migrations
└── Configurations/          # Entity configurations
```

## Key Features

### 🔄 Real-Time Communication
- **SignalR Hub** at `/hubs/plant`
- Broadcasts sensor updates to all connected clients
- Automatic reconnection with exponential backoff
- WebSocket and Server-Sent Events fallback

### 📊 Telemetry Processing
- Receives sensor data from ESP32 devices
- Intelligent watering logic based on soil moisture
- **EF Core 10 `ExecuteUpdateAsync`** for high-performance updates
- Creates watering logs and broadcasts updates

### 📈 Historical Analytics
- Time-series data aggregation (hourly/daily/weekly)
- Min/Max/Avg calculations for all sensor metrics
- Watering history and plant statistics
- Efficient LINQ queries with proper indexing

### 🔍 Smart Plant Search
- Integration with Perenual API (40,000+ plant species)
- Search by common name or scientific name
- Returns care requirements and optimal growing conditions
- C# 14 `field` keyword for property validation

### 🔐 Security
- JWT token authentication for users
- Separate device token authentication for ESP32
- Role-based authorization
- Secure password hashing with BCrypt
- HTTPS enforcement in production

## Quick Start

### Prerequisites

```bash
# Required
.NET 10 SDK
SQL Server (LocalDB, Express, or Full)
Visual Studio 2024 or VS Code

# Optional
Docker Desktop (for containerized development)
```

### Installation

1. **Clone the repository**
```bash
cd SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API
```

2. **Configure Database**

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartGardenDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

3. **Configure API Keys**

Add to `appsettings.json` or User Secrets:
```json
{
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-characters",
    "Issuer": "SmartGardenAPI",
    "Audience": "SmartGardenApp",
    "ExpiryMinutes": 1440
  },
  "PerenualApiKey": "your-perenual-api-key",
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

4. **Run Migrations**
```bash
cd ../SmartGarden.Data
dotnet ef database update --startup-project ../SmartGarden.API
```

5. **Run the API**
```bash
cd ../SmartGarden.API
dotnet run
```

The API will start at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `https://localhost:5001/swagger`

## API Endpoints

### Authentication
```http
POST   /api/auth/register          # Register new user
POST   /api/auth/login             # User login
POST   /api/auth/device/register   # Register ESP32 device
POST   /api/auth/device/login      # Device login
```

### Plants
```http
GET    /api/plants                 # Get all user plants
GET    /api/plants/{id}            # Get plant details
POST   /api/plants                 # Add new plant
PUT    /api/plants/{id}            # Update plant
DELETE /api/plants/{id}            # Delete plant
GET    /api/plants/search          # Search plants (Perenual API)
```

### Telemetry (Device Endpoints)
```http
POST   /api/telemetry              # Submit sensor readings
```

**Request Body:**
```json
{
  "plantId": 1,
  "soilMoisture": 45.5,
  "waterLevel": 75.0,
  "airTemp": 22.5,
  "airHumidity": 65.0,
  "lightLevel": 800,
  "airQuality": 95.0
}
```

**Response:**
```json
{
  "command": "WATER",
  "duration": 3000
}
```

### Analytics
```http
GET    /api/analytics/plant/{id}/historical   # Time-series data
GET    /api/analytics/plant/{id}/summary      # Plant statistics
```

### SignalR Events
```javascript
// Subscribe to real-time updates
connection.on('ReceiveUpdate', (update) => {
  console.log('Plant update:', update);
});
```

## Development

### Running with Hot Reload
```bash
dotnet watch run
```

### Running Tests
```bash
cd ../SmartGarden.Tests
dotnet test
```

### Code Quality
```bash
# Format code
dotnet format

# Analyze code
dotnet build /p:EnforceCodeStyleInBuild=true
```

## Key Implementation Details

### EF Core 10 ExecuteUpdateAsync Optimization

Traditional approach (loads entity, tracks changes):
```csharp
var plant = await _context.Plants.FindAsync(plantId);
plant.LastWateredDate = DateTime.UtcNow;
await _context.SaveChangesAsync();
```

**Optimized approach** (direct SQL UPDATE):
```csharp
await _context.Plants
    .Where(p => p.PlantId == plantId)
    .ExecuteUpdateAsync(setter => setter
        .SetProperty(p => p.LastWateredDate, DateTime.UtcNow));
```

**Performance**: 50-70% faster for bulk updates, no change tracking overhead.

### C# 14 Field Keyword for Validation

```csharp
public int SuggestedMoistureThreshold
{
    get => field;
    set => field = value < 0 ? 0 : value > 100 ? 100 : value;
}
```

### SignalR Broadcasting

```csharp
// In TelemetryController
private async Task BroadcastPlantUpdateAsync(...)
{
    var update = new PlantUpdateDto { ... };
    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", update);
}
```

### Intelligent Watering Logic

```csharp
var threshold = plant.SuggestedMoistureThreshold ?? 40;
var needsWatering = request.SoilMoisture < threshold;
var hasSufficientWater = request.WaterLevel > 5.0;

if (needsWatering && hasSufficientWater)
{
    // Calculate duration: 1-5 seconds based on moisture deficit
    var wateringDuration = CalculateWateringDuration(threshold);
    return Ok(new TelemetryResponseDto
    {
        Command = "WATER",
        Duration = wateringDuration
    });
}

return Ok(new TelemetryResponseDto { Command = "NONE" });
```

## Database

### Connection String Formats

**LocalDB** (Development):
```
Server=(localdb)\\mssqllocaldb;Database=SmartGardenDb;Trusted_Connection=true
```

**SQL Server Express**:
```
Server=localhost\\SQLEXPRESS;Database=SmartGardenDb;Trusted_Connection=true
```

**Azure SQL**:
```
Server=tcp:yourserver.database.windows.net,1433;Database=SmartGardenDb;User ID=yourusername;Password=yourpassword;Encrypt=true
```

### Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --startup-project ../SmartGarden.API

# Update database
dotnet ef database update --startup-project ../SmartGarden.API

# Rollback migration
dotnet ef database update PreviousMigrationName --startup-project ../SmartGarden.API

# Remove last migration
dotnet ef migrations remove --startup-project ../SmartGarden.API
```

## Configuration

### User Secrets (Recommended for Development)

```bash
cd SmartGarden.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Secret" "your-secret-key"
dotnet user-secrets set "PerenualApiKey" "your-api-key"
dotnet user-secrets set "EmailSettings:SenderPassword" "your-password"
```

### Environment Variables (Production)

```bash
export ConnectionStrings__DefaultConnection="your-connection-string"
export JwtSettings__Secret="your-secret-key"
export PerenualApiKey="your-api-key"
```

## Docker Support

### Build Image
```bash
docker build -t smartgarden-api -f Dockerfile .
```

### Run Container
```bash
docker run -d \
  -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  -e JwtSettings__Secret="your-secret" \
  --name smartgarden-api \
  smartgarden-api
```

### Docker Compose
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=SmartGardenDb;User=sa;Password=YourPassword123
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
```

## Troubleshooting

### Issue: Database Connection Failed
```bash
# Check SQL Server is running
# For LocalDB:
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb

# Test connection
dotnet ef database update --startup-project ../SmartGarden.API --verbose
```

### Issue: SignalR Connection Failed
- Ensure CORS is configured for your frontend origin
- Check firewall allows WebSocket connections
- Verify SignalR endpoint: `https://localhost:5001/hubs/plant`

### Issue: JWT Token Invalid
- Check token hasn't expired (default 24 hours)
- Verify JWT secret matches in configuration
- Ensure clock sync between client and server

## Performance Optimization

### Database Indexing
```csharp
// In entity configuration
builder.HasIndex(p => p.UserId);
builder.HasIndex(p => p.DeviceId);
builder.HasIndex(sr => new { sr.PlantId, sr.CreatedAt });
```

### Response Caching
```csharp
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public async Task<IActionResult> GetPlantStatistics(int id)
```

### Async Best Practices
- Always use `async/await` for I/O operations
- Use `ConfigureAwait(false)` in library code
- Prefer `ExecuteUpdateAsync` over `SaveChangesAsync` for bulk updates

## API Versioning

```csharp
// Example for future v2 endpoint
[ApiController]
[Route("api/v2/[controller]")]
public class PlantsV2Controller : ControllerBase
```

## Security Best Practices

1. **Always use HTTPS** in production
2. **Validate all input** with Data Annotations
3. **Sanitize user input** to prevent injection attacks
4. **Rate limit** API endpoints
5. **Use parameterized queries** (EF Core handles this)
6. **Store secrets** in Azure Key Vault or AWS Secrets Manager
7. **Enable CORS** only for trusted origins
8. **Log security events** (failed logins, unauthorized access)

## Monitoring & Logging

### Application Insights Integration
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

### Structured Logging
```csharp
_logger.LogInformation("Plant {PlantId} watered. Duration: {Duration}ms",
    plantId, duration);
```

## Contributing

1. Follow C# coding conventions
2. Write unit tests for new features
3. Update API documentation
4. Run code formatter before committing

## Related Documentation

- [Main Project Documentation](../../../../../PROJECT_DOCUMENTATION.md)
- [API Reference](../../../../../API_REFERENCE.md)
- [Frontend README](../../../../../ReactNativeApp/SmartGardenApp/README.md)
- [Firmware README](../../../../../FirmWare/README.md)

## Support

For issues, questions, or contributions:
- Check existing documentation
- Review API endpoints in Swagger UI
- Check logs in `Logs/` directory

---

**Version**: 1.0
**Last Updated**: November 2025
**Framework**: .NET 10
**License**: MIT
