# 🔒 SmartGarden Secure ESP32 Firmware

## Security Features

✅ **HTTPS/TLS** - All communication encrypted with SSL/TLS
✅ **Device JWT Authentication** - Each device has unique JWT token
✅ **HMAC-SHA256 Signing** - All sensor data is cryptographically signed
✅ **Secure Credential Storage** - Tokens encrypted in EEPROM
✅ **Automatic Token Refresh** - Tokens refresh before expiry
✅ **Rate Limiting Aware** - Respects backend rate limits
✅ **WiFi Auto-Reconnect** - Handles network interruptions

---

## Hardware Requirements

### Components:
- ESP-WROOM-32
- SHT21 (Temperature & Humidity sensor)
- BH1750 (Light sensor)
- Capacitive soil moisture sensor
- MQ-135 (Air quality sensor)
- HC-SR04 (Ultrasonic distance sensor)
- IRF520 MOSFET module (for pump control)
- 5V water pump
- MT3608 boost converter (5V → 6V for pump)
- Powerbank or 5V power supply

### Wiring:
```
ESP32 GPIO Pins:
- GPIO 34 → Soil Moisture Sensor (Analog)
- GPIO 26 → IRF520 MOSFET Signal Pin
- GPIO 35 → MQ-135 Air Quality (Analog)
- GPIO 32 → HC-SR04 TRIG
- GPIO 33 → HC-SR04 ECHO
- SDA (GPIO 21) → SHT21 SDA + BH1750 SDA
- SCL (GPIO 22) → SHT21 SCL + BH1750 SCL
```

---

## Installation

### 1. Install PlatformIO (Recommended) or Arduino IDE

**PlatformIO:**
```bash
pip install platformio
```

**Arduino IDE:**
- Download from arduino.cc
- Install ESP32 board support

### 2. Install Required Libraries

**Arduino IDE:**
Go to Sketch → Include Library → Manage Libraries, then install:
- WiFiClientSecure
- HTTPClient
- ArduinoJson (v6.x)
- SHT2x
- BH1750
- mbedtls

**PlatformIO:**
Libraries are auto-installed from `platformio.ini`

### 3. Configure WiFi and API

Edit `SecureESP32.ino`:

```cpp
#define WIFI_SSID "YOUR_WIFI_SSID"
#define WIFI_PASSWORD "YOUR_WIFI_PASSWORD"
#define API_BASE_URL "https://your-api-domain.com/api"
```

### 4. Add Your SSL Certificate

Replace the `rootCACertificate` variable with your server's certificate.

**For Let's Encrypt certificates:**
```bash
openssl s_client -showcerts -connect your-api-domain.com:443 < /dev/null 2>/dev/null | openssl x509 -outform PEM
```

Copy the output and replace in firmware.

### 5. Upload Firmware

**Arduino IDE:**
1. Select board: ESP32 Dev Module
2. Select port
3. Click Upload

**PlatformIO:**
```bash
pio run --target upload
```

---

## First Boot Registration Flow

1. **ESP32 starts** → Reads EEPROM for existing credentials
2. **No credentials found** → Initiates device registration
3. **Sends registration request** to `/api/device-auth/register`
   ```json
   {
     "macAddress": "AA:BB:CC:DD:EE:FF",
     "model": "ESP32-SmartGarden-v1",
     "firmwareVersion": "1.0.0",
     "serialNumber": "123456"
   }
   ```
4. **Backend responds** with credentials:
   ```json
   {
     "deviceId": "uuid",
     "deviceToken": "jwt-token",
     "apiKey": "secure-api-key",
     "refreshToken": "refresh-token",
     "expiresIn": 86400,
     "requiresApproval": true
   }
   ```
5. **ESP32 saves credentials** to EEPROM
6. **Waits for user approval** in mobile app
7. **Once approved** → Starts sending sensor data

---

## Operation Modes

### Normal Mode (Default)
- Reads sensors every **15 minutes**
- Sends heartbeat every **1 minute**
- Checks for watering commands
- Auto-refreshes token before expiry

### Calibration Mode
- Activated from mobile app
- Reads sensors every **1 second**
- User calibrates min/max values
- Returns to normal mode when disabled

---

## Security Protocol

### Device Authentication:
1. Device registers and receives JWT token
2. All API requests include: `Authorization: Bearer <token>`
3. Token expires after 24 hours
4. Automatically refreshes 1 hour before expiry

### Message Signing (HMAC):
1. Create JSON payload: `{"soilMoisture": 45, "temp": 22, ...}`
2. Compute HMAC-SHA256: `signature = HMAC(payload, apiKey)`
3. Append signature to payload
4. Backend verifies signature before accepting data

### Rate Limiting:
- Backend allows **120 requests/hour** per device
- Heartbeat + sensor reading = ~2 requests/minute
- ESP32 tracks rate limits and backs off if needed

---

## Troubleshooting

### Device Won't Register
- ✅ Check WiFi credentials
- ✅ Verify API_BASE_URL is correct (HTTPS!)
- ✅ Ensure backend is running
- ✅ Check Serial Monitor for errors

### "Unauthorized" Errors
- Token may have expired
- Backend will auto-refresh on next request
- Check Serial Monitor for token refresh logs

### Sensor Readings Zero
- ✅ Check sensor wiring (I2C: SDA/SCL, Analog pins)
- ✅ Verify sensors are powered (3.3V or 5V as needed)
- ✅ Test sensors individually with example sketches

### Pump Not Working
- ✅ Check MOSFET connection (Signal to GPIO 26)
- ✅ Verify pump power supply (5-6V)
- ✅ Test MOSFET manually: `digitalWrite(26, HIGH);`
- ✅ Ensure diode is connected (flyback protection)

---

## Serial Monitor Output Example

```
===== SmartGarden Secure ESP32 =====
Firmware Version: 1.0.0
Connecting to WiFi...
WiFi connected!
IP address: 192.168.1.100
Sensors initialized
Secure HTTPS client configured
Device not registered. Starting registration...
POST https://api.smartgarden.com/api/device-auth/register
HTTP Code: 200
Device registered successfully!
Device ID: abc-123-def-456
Waiting for user approval...
Setup complete. Starting main loop...
Sending heartbeat...
POST https://api.smartgarden.com/api/device-auth/heartbeat
HTTP Code: 200
Heartbeat sent successfully
Reading sensors and sending data...
POST https://api.smartgarden.com/api/sensor
HTTP Code: 201
Sensor data sent successfully
```

---

## API Endpoints Used

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/device-auth/register` | POST | Initial device registration |
| `/device-auth/refresh-token` | POST | Refresh expired JWT token |
| `/device-auth/heartbeat` | POST | Update device online status |
| `/sensor` | POST | Send sensor readings (signed) |
| `/watering/device/{id}/commands` | GET | Check for watering commands |
| `/plants/{plantId}/calibration` | GET | Check calibration mode status |

---

## Security Best Practices

✅ **Never commit WiFi passwords or API keys to Git**
✅ **Use environment-specific configuration files**
✅ **Change default API keys after deployment**
✅ **Enable HTTPS on your backend (required!)**
✅ **Regularly update firmware for security patches**
✅ **Monitor device logs for unauthorized access attempts**

---

## Firmware Updates (OTA)

Future versions will support Over-The-Air (OTA) updates. For now:

1. Update code locally
2. Increment `FIRMWARE_VERSION`
3. Re-upload via USB

---

## Support & Issues

- Check Serial Monitor output at 115200 baud
- Review backend logs for API errors
- Open GitHub issue with:
  - Firmware version
  - Error messages
  - Serial Monitor logs
  - Backend API response codes

---

## License

MIT License - See LICENSE file for details
