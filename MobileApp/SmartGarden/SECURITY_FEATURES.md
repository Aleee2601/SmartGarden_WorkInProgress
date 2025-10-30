# SmartGarden Security & Features Update

This document describes the newly implemented security features and enhancements added to the SmartGarden backend API.

## Overview of New Features

1. **JWT Authentication & Authorization**
2. **CORS Configuration**
3. **Real-time Automatic Watering**
4. **API Rate Limiting**

---

## 1. JWT Authentication & Authorization

### What Was Added

- JWT (JSON Web Token) based authentication system
- User registration and login endpoints
- Secure password hashing (SHA256)
- Token-based authorization for protected endpoints

### New API Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123",
  "name": "John Doe"
}
```

**Response:**
```json
{
  "userId": 1,
  "email": "user@example.com",
  "name": "John Doe",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-06T10:30:00Z"
}
```

#### POST `/api/auth/login`
Login with existing credentials.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123"
}
```

**Response:**
```json
{
  "userId": 1,
  "email": "user@example.com",
  "name": "John Doe",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-06T10:30:00Z"
}
```

#### GET `/api/auth/exists?email=user@example.com`
Check if a user exists by email.

**Response:**
```json
{
  "exists": true
}
```

### Protected Endpoints

The following endpoints now require authentication (JWT token in Authorization header):

- **PlantController**: All endpoints (`GET /api/plant`, `POST /api/plant`, etc.)
- **WateringController**: All endpoints (`POST /api/plants/{plantId}/watering`)
- **SensorController**: `GET /api/plants/{plantId}/sensor/readings`

**Exception**: `POST /api/plants/{plantId}/sensor/readings` remains publicly accessible to allow ESP32 devices to send sensor data without authentication.

### How to Use Authentication

1. **Register or Login** to get a JWT token
2. **Include the token** in subsequent requests:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

### Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "SmartGardenSecretKey2024!@#$%^&*()_+1234567890",
    "Issuer": "SmartGardenAPI",
    "Audience": "SmartGardenClient",
    "ExpirationDays": 7
  }
}
```

**IMPORTANT**: Change the `SecretKey` in production to a secure, randomly generated value.

---

## 2. CORS Configuration

### What Was Added

Cross-Origin Resource Sharing (CORS) policy to allow frontend applications from specific origins to access the API.

### Configuration

CORS settings in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:5000",
      "http://localhost:8080"
    ]
  }
}
```

### How to Configure

1. Add your frontend application's URL to the `AllowedOrigins` array
2. The API will accept requests from these origins with credentials
3. All HTTP methods and headers are allowed for configured origins

### Features

- **AllowCredentials**: Enables cookies and authentication headers
- **AllowAnyMethod**: Permits GET, POST, PUT, DELETE, etc.
- **AllowAnyHeader**: Accepts any request headers

---

## 3. Real-time Automatic Watering

### What Was Added

A background service that continuously monitors soil moisture levels and automatically waters plants when thresholds are exceeded.

### How It Works

1. **Runs Every 5 Minutes**: The service checks all plants every 5 minutes
2. **User Settings Based**: Only activates for users who enable auto-watering
3. **Threshold Monitoring**: Waters plants when soil moisture falls below user-defined threshold
4. **Cooldown Period**: Prevents over-watering by enforcing a 2-hour cooldown between watering events
5. **Data Freshness Check**: Only acts on sensor readings that are less than 30 minutes old

### Algorithm

```
For each user with AutoWateringEnabled:
  For each plant:
    1. Get latest sensor reading (must be < 30 min old)
    2. Check if soil moisture < threshold
    3. Check if last watering was > 2 hours ago
    4. If all conditions met → Water plant automatically
    5. Log watering event with mode="auto"
```

### User Settings

Users can control automatic watering through the `UserSettings` table:

```sql
AutoWateringEnabled     BOOLEAN  (default: false)
SoilMoistThreshold      FLOAT    (default: 30.0%)
DataReadIntervalMin     INT      (default: 15 minutes)
```

### ESP32 Integration

When auto-watering triggers, the service:
1. Sends HTTP POST to ESP32 at `http://{ESP32_IP}:5000/water`
2. Command payload: `"WATER AUTO"`
3. Logs the event in `WateringLogs` table

### Monitoring

Check application logs for auto-watering activity:
```
[Information] Found 2 users with auto watering enabled
[Information] Auto watering plant 5 (moisture: 25.3%, threshold: 30%)
[Information] Successfully watered plant 5
[Warning] Plant 3 was watered 45 minutes ago, skipping
```

---

## 4. API Rate Limiting

### What Was Added

IP-based rate limiting to prevent abuse and ensure fair usage of the API.

### Configuration

Rate limiting rules in `appsettings.json`:

```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      },
      {
        "Endpoint": "*",
        "Period": "1h",
        "Limit": 1000
      },
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "1m",
        "Limit": 5
      },
      {
        "Endpoint": "POST:/api/auth/register",
        "Period": "1h",
        "Limit": 3
      }
    ]
  }
}
```

### Rate Limit Rules

| Endpoint | Period | Limit | Purpose |
|----------|--------|-------|---------|
| All endpoints | 1 minute | 60 requests | General API usage |
| All endpoints | 1 hour | 1000 requests | Hourly cap |
| Login | 1 minute | 5 attempts | Brute-force protection |
| Register | 1 hour | 3 attempts | Anti-spam registration |

### ESP32 Exception

The ESP32 device (IP: `172.20.10.3`) has higher limits:

```json
{
  "Ip": "172.20.10.3",
  "Rules": [
    {
      "Endpoint": "*",
      "Period": "1m",
      "Limit": 200
    }
  ]
}
```

### Rate Limit Response

When rate limit is exceeded, the API returns:

```
HTTP 429 Too Many Requests
```

### How to Configure

1. **Adjust Limits**: Modify the `Limit` values in `appsettings.json`
2. **Add IP Exceptions**: Add specific IPs to `IpRateLimitPolicies` for higher limits
3. **Custom Endpoint Rules**: Add specific endpoints with custom limits

---

## Setup Instructions

### 1. Install Dependencies

```bash
cd MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API
dotnet restore
```

### 2. Update Configuration

Edit `appsettings.json`:

1. **Change JWT SecretKey** (production):
   ```json
   "SecretKey": "YOUR_SECURE_RANDOM_KEY_HERE_AT_LEAST_32_CHARS"
   ```

2. **Add CORS Origins** (your frontend URLs):
   ```json
   "AllowedOrigins": [
     "https://yourdomain.com",
     "https://app.yourdomain.com"
   ]
   ```

3. **Configure Rate Limits** (adjust as needed)

### 3. Run Migrations (if needed)

```bash
cd SmartGarden.Data
dotnet ef migrations add AddSecurityFeatures
dotnet ef database update
```

### 4. Build and Run

```bash
cd SmartGarden.API
dotnet build
dotnet run
```

### 5. Test Authentication

The API now serves Swagger UI at the root URL (`http://localhost:5000`):

1. Open browser to `http://localhost:5000`
2. Click "Authorize" button in Swagger UI
3. Register a new user via `/api/auth/register`
4. Copy the returned token
5. Click "Authorize" and enter: `Bearer <your-token>`
6. Test protected endpoints

---

## Security Best Practices

### For Production Deployment

1. **JWT Secret Key**
   - Generate a strong, random key (min 32 characters)
   - Store in environment variables or Azure Key Vault
   - Never commit to source control

2. **CORS Origins**
   - Only add trusted frontend domains
   - Never use `*` (allow all origins) in production
   - Use HTTPS URLs in production

3. **Rate Limiting**
   - Adjust limits based on expected traffic
   - Monitor logs for rate limit violations
   - Consider stricter limits for sensitive endpoints

4. **Password Security**
   - Current implementation uses SHA256
   - Consider upgrading to bcrypt or Argon2 for production
   - Enforce password complexity requirements

5. **HTTPS**
   - Always use HTTPS in production
   - Configure SSL certificates
   - Redirect HTTP to HTTPS

6. **API Keys for ESP32**
   - Consider adding API key authentication for ESP32
   - Store API key in ESP32 firmware
   - Validate API key in sensor reading endpoint

---

## File Changes Summary

### New Files Created

```
SmartGarden.Core/DTOs/
  ├─ RegisterDto.cs
  ├─ LoginDto.cs
  └─ AuthResponseDto.cs

SmartGarden.Core/Interfaces/
  └─ IAuthService.cs

SmartGarden.API/Services/
  ├─ AuthService.cs
  └─ AutoWateringBackgroundService.cs

SmartGarden.API/Controllers/
  └─ AuthController.cs
```

### Modified Files

```
SmartGarden.API/
  ├─ Program.cs                    (JWT, CORS, Rate Limiting)
  ├─ appsettings.json              (Configuration)
  ├─ SmartGarden.API.csproj        (NuGet packages)
  └─ Controllers/
      ├─ PlantController.cs        (Authorization)
      ├─ SensorController.cs       (Authorization)
      └─ WateringController.cs     (Authorization)
```

---

## Testing Checklist

- [ ] Register new user successfully
- [ ] Login with correct credentials
- [ ] Login fails with incorrect credentials
- [ ] Protected endpoints require authentication
- [ ] JWT token works in Swagger UI
- [ ] ESP32 can still post sensor readings without auth
- [ ] Auto-watering triggers when threshold exceeded
- [ ] Auto-watering respects cooldown period
- [ ] Rate limiting blocks after limit exceeded
- [ ] CORS allows requests from configured origins
- [ ] Rate limit is higher for ESP32 IP

---

## Troubleshooting

### Issue: 401 Unauthorized

**Cause**: Missing or invalid JWT token

**Solution**:
- Ensure you're logged in and have a valid token
- Check Authorization header: `Bearer <token>`
- Verify token hasn't expired (7 days)

### Issue: 429 Too Many Requests

**Cause**: Rate limit exceeded

**Solution**:
- Wait for the rate limit period to reset
- Adjust limits in `appsettings.json` if needed
- Check if you're being blocked by IP

### Issue: CORS Error in Browser

**Cause**: Frontend origin not in allowed list

**Solution**:
- Add your frontend URL to `Cors:AllowedOrigins` in `appsettings.json`
- Restart the API

### Issue: Auto-watering Not Working

**Cause**: User settings not configured

**Solution**:
- Ensure `AutoWateringEnabled = true` in UserSettings
- Set appropriate `SoilMoistThreshold`
- Check that sensor readings are recent (< 30 min)
- Check application logs for details

---

## API Endpoints Summary

### Authentication (Public)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login
- `GET /api/auth/exists` - Check if user exists

### Plants (Protected)
- `GET /api/plant` - Get all plants
- `GET /api/plant/{id}` - Get plant by ID
- `POST /api/plant` - Create plant
- `DELETE /api/plant/{id}` - Delete plant

### Sensors
- `GET /api/plants/{plantId}/sensor/readings` - Get readings (Protected)
- `POST /api/plants/{plantId}/sensor/readings` - Add reading (Public for ESP32)

### Watering (Protected)
- `POST /api/plants/{plantId}/watering` - Water plant

---

## Next Steps & Recommendations

1. **Password Hashing**: Upgrade to bcrypt or Argon2
2. **API Keys**: Add API key authentication for ESP32
3. **User Management**: Add endpoints for user profile updates
4. **Plant Ownership**: Filter plants by authenticated user
5. **Notifications**: Add push notifications for low moisture
6. **Audit Logging**: Log all authentication attempts
7. **2FA**: Consider two-factor authentication
8. **Email Verification**: Verify email addresses on registration

---

## Support

For issues or questions, please refer to the main project documentation or contact the development team.
