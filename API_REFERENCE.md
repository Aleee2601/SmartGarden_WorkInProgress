# SmartGarden API Reference

**Version:** 2.0
**Base URL:** `http://localhost:5000/api` (Development)
**Production URL:** `https://api.smartgarden.com/api`

---

## Table of Contents

1. [Authentication](#authentication)
2. [Plants](#plants)
3. [Telemetry](#telemetry)
4. [Analytics](#analytics)
5. [Devices](#devices)
6. [Sensors](#sensors)
7. [Watering](#watering)
8. [Alerts](#alerts)
9. [Device Authentication](#device-authentication)
10. [Error Codes](#error-codes)

---

## Authentication

### Register User

**Endpoint:** `POST /api/auth/register`

**Description:** Create a new user account.

**Request Body:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Validation Rules:**
- `username`: 3-100 characters, required
- `email`: Valid email format, unique, required
- `password`: Minimum 8 characters, required

**Response:** `200 OK`
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

**Error Responses:**
- `400 Bad Request` - Validation errors
- `409 Conflict` - Email already exists

---

### Login

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Response:** `200 OK`
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

**Error Responses:**
- `400 Bad Request` - Missing credentials
- `401 Unauthorized` - Invalid credentials

---

### Get Profile

**Endpoint:** `GET /api/auth/profile`

**Authentication:** Required (Bearer token)

**Response:** `200 OK`
```json
{
  "userId": 1,
  "username": "john_doe",
  "email": "john@example.com",
  "createdAt": "2025-01-01T00:00:00Z"
}
```

---

## Plants

### Get All Plants

**Endpoint:** `GET /api/plant`

**Authentication:** Required

**Description:** Get all plants for the authenticated user.

**Response:** `200 OK`
```json
[
  {
    "plantId": 1,
    "nickname": "My Kitchen Basil",
    "speciesName": "Ocimum basilicum",
    "roomName": "Kitchen",
    "isOutdoor": false,
    "dateAcquired": "2025-01-15T00:00:00Z",
    "lastWateredDate": "2025-11-25T08:30:00Z",
    "latestSensorReading": {
      "soilMoisture": 45.5,
      "airTemp": 22.3,
      "waterLevel": 85.0,
      "lightLevel": 1200.5,
      "createdAt": "2025-11-25T10:00:00Z"
    }
  }
]
```

---

### Get Plant by ID

**Endpoint:** `GET /api/plant/{id}`

**Authentication:** Required

**Parameters:**
- `id` (path, integer) - Plant ID

**Response:** `200 OK`
```json
{
  "plantId": 1,
  "nickname": "My Kitchen Basil",
  "species": {
    "speciesId": 10,
    "commonName": "Basil",
    "scientificName": "Ocimum basilicum"
  },
  "soilType": {
    "soilTypeId": 1,
    "name": "Potting Mix"
  },
  "roomName": "Kitchen",
  "isOutdoor": false,
  "dateAcquired": "2025-01-15T00:00:00Z",
  "thresholds": [
    {
      "minSoilMoisture": 40.0,
      "maxSoilMoisture": 70.0,
      "minAirTemp": 18.0,
      "maxAirTemp": 28.0,
      "isActive": true
    }
  ]
}
```

**Error Responses:**
- `404 Not Found` - Plant not found

---

### Create Plant

**Endpoint:** `POST /api/plant`

**Authentication:** Required

**Request Body:**
```json
{
  "nickname": "My Kitchen Basil",
  "speciesId": 10,
  "soilTypeId": 1,
  "roomName": "Kitchen",
  "isOutdoor": false,
  "dateAcquired": "2025-01-15T00:00:00Z",
  "deviceId": 1
}
```

**Response:** `201 Created`
```json
{
  "plantId": 1,
  "nickname": "My Kitchen Basil",
  "message": "Plant created successfully"
}
```

---

### Smart Plant Search

**Endpoint:** `GET /api/plant/search`

**Authentication:** Required

**Query Parameters:**
- `q` (string, required) - Search query

**Example:** `GET /api/plant/search?q=basil`

**Response:** `200 OK`
```json
[
  {
    "externalId": "12345",
    "commonName": "Sweet Basil",
    "scientificName": "Ocimum basilicum",
    "imageUrl": "https://cdn.example.com/basil.jpg",
    "wateringDescription": "Average",
    "sunlightDescription": "Full sun",
    "suggestedMoistureThreshold": 60
  }
]
```

---

### Create Plant from Search

**Endpoint:** `POST /api/plant/from-search`

**Authentication:** Required

**Request Body:**
```json
{
  "nickname": "My Kitchen Basil",
  "speciesName": "Ocimum basilicum",
  "imageUrl": "https://cdn.example.com/basil.jpg",
  "minMoistureThreshold": 60,
  "roomName": "Kitchen",
  "isOutdoor": false,
  "deviceId": 1
}
```

**Response:** `201 Created`
```json
{
  "plantId": 1,
  "nickname": "My Kitchen Basil",
  "message": "Plant created from search successfully"
}
```

---

### Update Plant

**Endpoint:** `PUT /api/plant/{id}`

**Authentication:** Required

**Request Body:**
```json
{
  "nickname": "Updated Name",
  "roomName": "Living Room"
}
```

**Response:** `200 OK`

---

### Delete Plant

**Endpoint:** `DELETE /api/plant/{id}`

**Authentication:** Required

**Description:** Soft delete (sets IsDeleted flag).

**Response:** `204 No Content`

---

## Telemetry

### Submit Telemetry Data

**Endpoint:** `POST /api/telemetry`

**Authentication:** Required (Device token)

**Description:** ESP32 devices send sensor data and receive watering commands.

**Request Body:**
```json
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

**Validation Rules:**
- `deviceId`: Required, integer
- `soilMoisture`: Required, 0-100
- `tankLevel`: Required, 0-100
- `airTemp`: Optional, double
- `airHumidity`: Optional, 0-100
- `lightLevel`: Optional, double
- `airQuality`: Optional, double

**Response (Watering Needed):** `200 OK`
```json
{
  "command": "WATER",
  "duration": 5,
  "message": "Soil moisture low (35.5% < 40.0%)",
  "nextCheckInSeconds": 300
}
```

**Response (No Watering):** `200 OK`
```json
{
  "command": "SLEEP",
  "message": "Soil moisture adequate (45.2% >= 40.0%)",
  "nextCheckInSeconds": 300
}
```

**Response (Tank Low):** `200 OK`
```json
{
  "command": "SLEEP",
  "message": "Water tank low (3.5% <= 5%)",
  "nextCheckInSeconds": 300
}
```

**Error Responses:**
- `400 Bad Request` - Invalid data
- `404 Not Found` - Device not registered
- `401 Unauthorized` - Invalid device token

---

### Health Check

**Endpoint:** `GET /api/telemetry/health`

**Authentication:** Not required

**Response:** `200 OK`
```json
{
  "status": "healthy",
  "timestamp": "2025-11-25T10:30:00Z",
  "message": "Telemetry controller is operational"
}
```

---

## Analytics

### Get Historical Data

**Endpoint:** `GET /api/analytics/plant/{id}/historical`

**Authentication:** Required

**Query Parameters:**
- `startDate` (datetime, optional) - Start date (default: 7 days ago)
- `endDate` (datetime, optional) - End date (default: now)
- `interval` (string, optional) - "hourly", "daily", "weekly" (default: "hourly")

**Example:**
```
GET /api/analytics/plant/1/historical
  ?startDate=2025-11-17T00:00:00Z
  &endDate=2025-11-24T23:59:59Z
  &interval=daily
```

**Response:** `200 OK`
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
      "minAirTemp": 20.1,
      "maxAirTemp": 25.3,
      "avgAirHumidity": 65.0,
      "avgLightLevel": 1200.5,
      "avgWaterLevel": 85.0,
      "readingCount": 24
    }
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
    "avgAirTemp": 22.5,
    "avgLightLevel": 1200.5,
    "totalReadings": 168,
    "totalWaterings": 4,
    "daysAboveThreshold": 5,
    "daysBelowThreshold": 2
  }
}
```

---

### Get Plant Summary

**Endpoint:** `GET /api/analytics/plant/{id}/summary`

**Authentication:** Required

**Query Parameters:**
- `days` (integer, optional) - Number of days to look back (default: 7)

**Example:** `GET /api/analytics/plant/1/summary?days=7`

**Response:** `200 OK`
```json
{
  "avgSoilMoisture": 45.2,
  "minSoilMoisture": 25.0,
  "maxSoilMoisture": 65.0,
  "avgAirTemp": 22.5,
  "avgLightLevel": 1200.5,
  "totalReadings": 168,
  "totalWaterings": 4,
  "daysAboveThreshold": 5,
  "daysBelowThreshold": 2
}
```

---

## Devices

### Get All Devices

**Endpoint:** `GET /api/device`

**Authentication:** Required

**Response:** `200 OK`
```json
[
  {
    "deviceId": 1,
    "deviceName": "ESP32-Kitchen",
    "macAddress": "AA:BB:CC:DD:EE:FF",
    "ipAddress": "192.168.1.100",
    "isOnline": true,
    "lastSeen": "2025-11-25T10:25:00Z",
    "batteryLevel": 85,
    "signalStrength": -45,
    "firmwareVersion": "1.0.0",
    "plantId": 1,
    "plantNickname": "My Kitchen Basil"
  }
]
```

---

### Register Device

**Endpoint:** `POST /api/device`

**Authentication:** Required

**Request Body:**
```json
{
  "deviceName": "ESP32-Kitchen",
  "macAddress": "AA:BB:CC:DD:EE:FF",
  "model": "ESP32 DevKit",
  "serialNumber": "ESP32-2024-001",
  "firmwareVersion": "1.0.0"
}
```

**Response:** `201 Created`
```json
{
  "deviceId": 1,
  "deviceName": "ESP32-Kitchen",
  "deviceToken": "generated-device-jwt-token",
  "message": "Device registered successfully. Awaiting approval."
}
```

---

### Approve Device

**Endpoint:** `POST /api/device/{id}/approve`

**Authentication:** Required

**Description:** Approve a pending device registration.

**Response:** `200 OK`
```json
{
  "message": "Device approved successfully"
}
```

---

## Sensors

### Get Latest Reading

**Endpoint:** `GET /api/sensor/{plantId}/latest`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "readingId": 12345,
  "plantId": 1,
  "soilMoisture": 45.5,
  "airTemp": 22.3,
  "airHumidity": 65.2,
  "lightLevel": 1200.5,
  "waterLevel": 85.0,
  "airQuality": 50.0,
  "createdAt": "2025-11-25T10:30:00Z"
}
```

---

### Get Reading History

**Endpoint:** `GET /api/sensor/{plantId}/history`

**Authentication:** Required

**Query Parameters:**
- `startDate` (datetime, optional)
- `endDate` (datetime, optional)
- `limit` (integer, optional) - Max records to return

**Response:** `200 OK`
```json
[
  {
    "readingId": 12345,
    "soilMoisture": 45.5,
    "airTemp": 22.3,
    "createdAt": "2025-11-25T10:30:00Z"
  }
]
```

---

## Watering

### Manual Watering

**Endpoint:** `POST /api/watering/{plantId}/manual`

**Authentication:** Required

**Request Body:**
```json
{
  "durationSec": 30
}
```

**Response:** `200 OK`
```json
{
  "message": "Watering started for 30 seconds",
  "wateringId": 123
}
```

---

### Configure Auto-Watering

**Endpoint:** `PUT /api/watering/{plantId}/auto`

**Authentication:** Required

**Request Body:**
```json
{
  "enabled": true,
  "intensity": 3
}
```

**Response:** `200 OK`
```json
{
  "message": "Auto-watering configured successfully"
}
```

---

### Get Watering Logs

**Endpoint:** `GET /api/watering/{plantId}/logs`

**Authentication:** Required

**Query Parameters:**
- `startDate` (datetime, optional)
- `endDate` (datetime, optional)

**Response:** `200 OK`
```json
[
  {
    "wateringId": 123,
    "plantId": 1,
    "durationSec": 5,
    "mode": "Auto",
    "createdAt": "2025-11-25T08:30:00Z"
  }
]
```

---

## Error Codes

### Standard HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Successful request |
| 201 | Created | Resource created successfully |
| 204 | No Content | Successful deletion |
| 400 | Bad Request | Invalid input data |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Duplicate resource |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server error |

### Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."],
    "Password": ["Password must be at least 8 characters."]
  },
  "traceId": "00-trace-id-00"
}
```

---

## Rate Limiting

**Global Rate Limits:**
- General endpoints: 10 requests per second
- Login endpoint: 5 requests per minute
- Telemetry endpoint: 1 request per 5 seconds (per device)

**Headers:**
```
X-Rate-Limit-Limit: 10
X-Rate-Limit-Remaining: 7
X-Rate-Limit-Reset: 1700000000
```

**429 Response:**
```json
{
  "message": "Rate limit exceeded. Please try again later.",
  "retryAfter": 60
}
```

---

## Pagination

**Query Parameters:**
- `page` (integer) - Page number (default: 1)
- `pageSize` (integer) - Items per page (default: 20, max: 100)

**Response Headers:**
```
X-Total-Count: 150
X-Page-Number: 1
X-Page-Size: 20
X-Total-Pages: 8
```

---

## SignalR Real-Time Events

**Hub URL:** `/hubs/plant`

**Events:**

### ReceiveUpdate
Broadcast when new sensor data arrives.

```json
{
  "plantId": 1,
  "plantName": "My Basil",
  "soilMoisture": 45.5,
  "waterLevel": 85.0,
  "airTemp": 22.3,
  "airHumidity": 65.2,
  "lightLevel": 1200.5,
  "airQuality": 50.0,
  "timestamp": "2025-11-25T10:30:00Z",
  "isWatering": false
}
```

---

*For more information, visit the Swagger documentation at http://localhost:5000*
