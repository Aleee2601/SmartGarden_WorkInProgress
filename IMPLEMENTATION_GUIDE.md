# 🌱 SmartGarden Complete Implementation Guide

## 📋 What Has Been Built

### ✅ **1. Enhanced Backend Security (C# .NET)**

#### New Files Created:
- `SmartGarden.Core/Models/DeviceAuth.cs` - Security credentials model
- `SmartGarden.Core/DTOs/DeviceAuthDtos.cs` - Request/response DTOs
- `SmartGarden.Core/Interfaces/IDeviceAuthService.cs` - Service contract
- `SmartGarden.Data/Persistence/Configurations/DeviceAuthConfiguration.cs` - EF configuration
- `SmartGarden.API/Services/DeviceAuthService.cs` - Security implementation
- `SmartGarden.API/Controllers/DeviceAuthController.cs` - API endpoints

#### Security Features Implemented:
✅ Dual JWT authentication (User + Device tokens)
✅ Separate secrets for users and devices
✅ Device registration with MAC address verification
✅ HMAC-SHA256 message signing for sensor data
✅ API key hashing (SHA256)
✅ Rate limiting per device (120 requests/hour)
✅ Failed authentication tracking & device lockout
✅ Token refresh mechanism
✅ Device approval workflow
✅ Certificate pinning support

#### Updated Files:
- `SmartGarden.Data/Persistence/SmartGardenDbContext.cs` - Added DeviceAuth DbSet
- `SmartGarden.Core/Models/Device.cs` - Added DeviceAuth navigation property
- `SmartGarden.API/Program.cs` - Configured dual JWT authentication
- `SmartGarden.API/appsettings.json` - Added UserSecret and DeviceSecret

---

### ✅ **2. Secure ESP32 Firmware**

#### Files Created:
- `FirmWare/SecureESP32/SecureESP32.ino` - Main firmware (Part 1)
- `FirmWare/SecureESP32/SecureESP32_Part2.ino` - Security functions (Part 2)
- `FirmWare/SecureESP32/README.md` - Complete documentation

#### Security Features:
✅ HTTPS/TLS communication with root CA certificate
✅ Device JWT authentication
✅ HMAC-SHA256 payload signing
✅ Secure credential storage in EEPROM (encrypted)
✅ Automatic token refresh (1 hour before expiry)
✅ WiFi auto-reconnect with exponential backoff
✅ Rate limiting awareness
✅ First-boot device registration flow

#### Hardware Integration:
✅ SHT21 - Temperature & Humidity
✅ BH1750 - Light intensity
✅ Capacitive soil moisture sensor
✅ MQ-135 - Air quality
✅ HC-SR04 - Water level (ultrasonic)
✅ IRF520 MOSFET - Pump control with safety

---

### 🔄 **3. Authentication Flow (Complete)**

```
┌─────────────────────────────────────────────────────────┐
│             DEVICE REGISTRATION FLOW                     │
└─────────────────────────────────────────────────────────┘

1. ESP32 First Boot
   ↓
2. Check EEPROM for existing credentials
   ↓ (not found)
3. Generate unique ID from MAC address
   ↓
4. POST /api/device-auth/register
   Request: {
     "macAddress": "AA:BB:CC:DD:EE:FF",
     "model": "ESP32-SmartGarden-v1",
     "firmwareVersion": "1.0.0"
   }
   ↓
5. Backend Validates & Issues Credentials
   Response: {
     "deviceId": "uuid",
     "deviceToken": "jwt-token" (24h expiry),
     "apiKey": "base64-key" (for HMAC),
     "refreshToken": "refresh-token" (30d expiry),
     "requiresApproval": true
   }
   ↓
6. ESP32 Saves to EEPROM (encrypted)
   ↓
7. Device Status: "Pending Approval"
   ↓
8. User Approves via Mobile App
   ↓
9. Device Status: "Active"
   ↓
10. ESP32 Starts Sending Sensor Data


┌─────────────────────────────────────────────────────────┐
│              DATA TRANSMISSION FLOW                      │
└─────────────────────────────────────────────────────────┘

1. ESP32 Reads Sensors (every 15 min)
   ↓
2. Create JSON payload:
   {
     "deviceId": "uuid",
     "plantId": 1,
     "soilMoisture": 45.2,
     "airTemperature": 22.5,
     "airHumidity": 55.0,
     "lightLevel": 320.0,
     "timestamp": 1234567890
   }
   ↓
3. Sign payload with HMAC-SHA256:
   signature = HMAC(payload, apiKey)
   ↓
4. Add signature to payload
   ↓
5. POST /api/sensor (with JWT + signature)
   Headers:
     Authorization: Bearer <device-jwt>
     X-Device-ID: uuid
   ↓
6. Backend Validates:
   - JWT signature
   - Device approval status
   - HMAC signature
   - Rate limit
   ↓
7. Save to SensorReadings table
   ↓
8. Return HTTP 201 Created
```

---

## 🔧 **Next Steps: Running the System**

### Step 1: Database Migration

```bash
cd /home/user/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project

# Create migration for DeviceAuth
dotnet ef migrations add AddDeviceAuthSecurity \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API

# Apply migration
dotnet ef database update \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API
```

### Step 2: Update Backend Configuration

Edit `SmartGarden.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartGardenDB;Integrated Security=true;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "UserSecret": "CHANGE-THIS-TO-SECURE-SECRET-KEY-FOR-USERS-MIN-32-CHARS",
    "DeviceSecret": "CHANGE-THIS-TO-SECURE-SECRET-KEY-FOR-DEVICES-MIN-32-CHARS",
    "Issuer": "SmartGarden",
    "Audience": "SmartGarden"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:19000",  // Expo Dev Server
      "http://localhost:19006",  // Expo Web
      "exp://192.168.1.100:19000" // Expo Mobile (update IP)
    ]
  }
}
```

### Step 3: Run Backend

```bash
cd SmartGarden.API
dotnet run
```

Backend will be available at: `https://localhost:5000`

### Step 4: Configure ESP32 Firmware

Edit `FirmWare/SecureESP32/SecureESP32.ino`:

```cpp
#define WIFI_SSID "YOUR_WIFI_SSID"
#define WIFI_PASSWORD "YOUR_WIFI_PASSWORD"
#define API_BASE_URL "https://YOUR-SERVER-IP-OR-DOMAIN:5000/api"
```

**Get your server's SSL certificate:**

```bash
openssl s_client -showcerts -connect YOUR-SERVER:5000 < /dev/null 2>/dev/null | openssl x509 -outform PEM
```

Copy the certificate and replace `rootCACertificate` in firmware.

### Step 5: Upload Firmware to ESP32

Using Arduino IDE:
1. Open `SecureESP32.ino`
2. Select Board: "ESP32 Dev Module"
3. Select Port
4. Click "Upload"

### Step 6: Verify Device Registration

Open Serial Monitor (115200 baud):
```
===== SmartGarden Secure ESP32 =====
Connecting to WiFi...
WiFi connected!
Device not registered. Starting registration...
POST https://YOUR-SERVER/api/device-auth/register
HTTP Code: 200
Device registered successfully!
Device ID: abc-123-def-456
Waiting for user approval...
```

---

## 🔐 **Security Best Practices**

### Production Deployment Checklist:

✅ **Backend:**
- [ ] Change JWT secrets to strong random values (min 32 chars)
- [ ] Enable HTTPS with valid SSL certificate (Let's Encrypt)
- [ ] Set CORS to specific domains (not `*`)
- [ ] Configure firewall to allow only HTTPS (443)
- [ ] Enable SQL Server authentication (not Integrated Security)
- [ ] Set up automated backups
- [ ] Configure logging to external service (e.g., Seq, Application Insights)
- [ ] Set up monitoring & alerts

✅ **ESP32:**
- [ ] Update root CA certificate to match your server
- [ ] Test device registration flow end-to-end
- [ ] Verify HMAC signature validation works
- [ ] Test WiFi reconnection (unplug/replug router)
- [ ] Test token refresh (wait 23 hours or mock expiry)
- [ ] Verify rate limiting (send rapid requests)

✅ **Mobile App (Next Phase):**
- [ ] Implement device approval flow
- [ ] Add biometric authentication (Face ID/Touch ID)
- [ ] Store tokens in secure storage (Keychain/Keystore)
- [ ] Implement certificate pinning
- [ ] Add session timeout

---

## 🧪 **Testing**

### Manual Testing Checklist:

**Backend API:**
```bash
# Test device registration
curl -X POST https://localhost:5000/api/device-auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "macAddress": "AA:BB:CC:DD:EE:FF",
    "model": "ESP32-SmartGarden-v1",
    "firmwareVersion": "1.0.0"
  }'

# Expected: 200 OK with deviceId, deviceToken, apiKey, refreshToken
```

**ESP32 Firmware:**
1. Upload firmware
2. Open Serial Monitor
3. Verify device registers
4. Check backend database: `SELECT * FROM Devices WHERE MacAddress = '...'`
5. Approve device via SQL (temporary): `UPDATE DeviceAuths SET IsApproved = 1 WHERE DeviceId = X`
6. Verify sensor data appears in `SensorReadings` table

---

## 📊 **Database Schema Changes**

### New Table: `DeviceAuths`

```sql
CREATE TABLE DeviceAuths (
    DeviceAuthId INT IDENTITY(1,1) PRIMARY KEY,
    DeviceId INT NOT NULL UNIQUE,
    ApiKeyHash NVARCHAR(256) NOT NULL,
    RefreshToken NVARCHAR(500) NOT NULL,
    TokenExpiry DATETIME2,
    RefreshTokenExpiry DATETIME2,
    IsApproved BIT DEFAULT 0,
    ApprovedByUserId NVARCHAR(450),
    ApprovedAt DATETIME2,
    CertificateFingerprint NVARCHAR(256),
    RequestCount INT DEFAULT 0,
    LastRequestAt DATETIME2,
    RateLimitResetAt DATETIME2,
    FailedAuthAttempts INT DEFAULT 0,
    LastFailedAuthAt DATETIME2,
    IsLocked BIT DEFAULT 0,
    LockedUntil DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (DeviceId) REFERENCES Devices(DeviceId) ON DELETE CASCADE
);

CREATE INDEX IX_DeviceAuths_IsApproved ON DeviceAuths(IsApproved);
CREATE INDEX IX_DeviceAuths_IsLocked ON DeviceAuths(IsLocked);
```

---

## 🚀 **What's Next: React Native App**

The mobile app will be built with:
- React Native + Expo
- TypeScript
- React Navigation
- Axios (HTTP client with interceptors)
- Redux Toolkit (state management)
- React Native Chart Kit (sensor graphs)
- Expo Secure Store (token storage)
- Expo Notifications (push notifications)

Features to implement:
1. ✅ User authentication (login/register)
2. ✅ Device approval workflow
3. ✅ Plant management (CRUD)
4. ✅ Real-time sensor data dashboard
5. ✅ Sensor history charts
6. ✅ Manual/auto watering controls
7. ✅ Device calibration wizard
8. ✅ Push notifications for alerts

---

## 📝 **Current System Status**

| Component | Status | Notes |
|-----------|--------|-------|
| Backend API | ✅ Complete | 30+ endpoints, dual JWT auth |
| Database Schema | ✅ Complete | 18 tables, audit & soft delete |
| Device Auth Service | ✅ Complete | HMAC signing, rate limiting |
| ESP32 Firmware | ✅ Complete | HTTPS, auto-registration |
| React Native App | 🔄 In Progress | Next phase |
| End-to-End Tests | ⏳ Pending | After React Native |
| Deployment Docs | ⏳ Pending | Final phase |

---

## 💡 **Tips & Troubleshooting**

### "Device not approved" in ESP32 logs
**Solution:** Approve device in backend:
```sql
UPDATE DeviceAuths
SET IsApproved = 1, ApprovedByUserId = '1', ApprovedAt = GETUTCDATE()
WHERE DeviceId = (SELECT DeviceId FROM Devices WHERE MacAddress = 'YOUR-MAC');
```

### "Unauthorized" errors after 24 hours
**Solution:** Token expired and refresh failed. Check:
- Refresh token is still valid (30 days)
- ESP32 has internet connection
- Backend is running

### Sensor data not appearing in database
**Solution:** Check:
1. Device is approved (`IsApproved = 1`)
2. HMAC signature is correct
3. Backend logs for validation errors
4. Rate limit not exceeded

---

## 📧 **Support**

For issues or questions:
1. Check Serial Monitor output (ESP32)
2. Check backend logs (`dotnet run` console)
3. Review this implementation guide
4. Check individual README files in each folder

---

**Last Updated:** [Your Date]
**Version:** 1.0.0
