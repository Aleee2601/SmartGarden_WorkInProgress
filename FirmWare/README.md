# SmartGarden ESP32 Firmware 🌱

## Overview

This directory contains the firmware for ESP32-based plant monitoring devices in the SmartGarden IoT system. The firmware collects sensor data, communicates with the backend API, and controls automated watering systems.

## Firmware Variants

### 📁 SecureESP32 (Recommended)
Production-ready firmware with secure device authentication, JWT token management, and HTTPS support.

**Features:**
- Secure device registration and authentication
- JWT token management with refresh
- HTTPS/TLS communication
- OTA (Over-The-Air) firmware updates
- Watchdog timer for reliability
- Deep sleep mode for power saving
- Error recovery and automatic reconnection

**Use Cases:**
- Production deployments
- Multiple device installations
- Battery-powered devices
- Security-sensitive environments

### 📁 EspCode
Basic ESP32 firmware for testing and development.

**Features:**
- Simple HTTP communication
- Basic sensor reading
- Manual watering control
- Serial debugging

**Use Cases:**
- Initial prototyping
- Sensor calibration
- Testing individual components
- Learning ESP32 programming

### 📁 ModulWifiCod
WiFi module configuration and testing code.

**Features:**
- WiFi connection management
- Network scanning
- Connection stability testing
- AP mode for initial setup

**Use Cases:**
- Network configuration
- WiFi troubleshooting
- Signal strength testing

### 📁 ArduinoCOd
Arduino-compatible code for beginners and simple setups.

**Features:**
- Arduino IDE compatibility
- Simplified code structure
- Basic sensor integration

**Use Cases:**
- Arduino IDE users
- Educational purposes
- Simple single-plant monitoring

## Hardware Requirements

### ESP32 Microcontroller
- **Model:** ESP32-DevKitC, ESP32-WROOM-32, or compatible
- **Flash:** 4MB minimum (8MB recommended for OTA)
- **RAM:** 520KB
- **WiFi:** 802.11 b/g/n
- **Bluetooth:** Bluetooth 4.2 (not used currently)
- **Operating Voltage:** 3.3V
- **Power:** 500mA minimum (1A recommended)

### Sensors

#### Required Sensors:
1. **Soil Moisture Sensor**
   - Capacitive soil moisture sensor v1.2
   - Analog output: 0-3.3V
   - Pin: GPIO 34 (ADC1_CH6)
   - Calibration range: 0-100%

2. **Water Level Sensor**
   - Analog water level sensor
   - Output: 0-3.3V
   - Pin: GPIO 35 (ADC1_CH7)
   - Range: 0-100%

#### Optional Sensors:
3. **DHT22 Temperature & Humidity Sensor**
   - Digital sensor
   - Pin: GPIO 15
   - Range: -40°C to 80°C, 0-100% RH
   - Accuracy: ±0.5°C, ±2% RH

4. **BH1750 Light Sensor**
   - I2C digital light sensor
   - Pins: SDA (GPIO 21), SCL (GPIO 22)
   - Range: 1-65535 lux
   - Accuracy: ±20%

5. **MQ-135 Air Quality Sensor**
   - Analog gas sensor
   - Pin: GPIO 32 (ADC1_CH4)
   - Detects: CO2, NH3, NOx, alcohol, benzene
   - Warm-up time: 24 hours for accurate readings

### Actuators

1. **Water Pump**
   - 3-6V DC submersible pump
   - Control: Relay module or MOSFET
   - Pin: GPIO 26
   - Current: 100-200mA
   - Flow rate: 80-120 L/hour

2. **Relay Module (Optional)**
   - 1-channel 5V relay module
   - Control: GPIO 26
   - Load capacity: 10A @ 250VAC / 10A @ 30VDC
   - Isolation: Optocoupler

### Power Supply

**Development:**
- USB power (5V 1A)
- USB cable connected to computer

**Production:**
- 5V 2A power adapter
- Battery pack: 3.7V LiPo (2000-5000mAh) + TP4056 charging module
- Solar panel: 6V 2W + battery backup

## Pin Configuration

### SecureESP32 Default Pinout

```
// Sensor Pins
#define SOIL_MOISTURE_PIN    34  // ADC1_CH6
#define WATER_LEVEL_PIN      35  // ADC1_CH7
#define DHT_PIN              15  // Digital
#define AIR_QUALITY_PIN      32  // ADC1_CH4
#define SDA_PIN              21  // I2C Data
#define SCL_PIN              22  // I2C Clock

// Actuator Pins
#define PUMP_PIN             26  // PWM capable

// Status LED
#define LED_PIN               2  // Built-in LED

// Button (optional)
#define BUTTON_PIN           0   // Built-in BOOT button
```

### ADC Configuration
```cpp
// ADC resolution: 12-bit (0-4095)
// ADC attenuation: 11dB (0-3.3V)
analogReadResolution(12);
analogSetAttenuation(ADC_11db);
```

## Quick Start

### Prerequisites

1. **Arduino IDE 2.0+** or **PlatformIO**
2. **ESP32 Board Support**
   - In Arduino IDE: File → Preferences → Additional Board Manager URLs
   - Add: `https://raw.githubusercontent.com/espressif/arduino-esp32/gh-pages/package_esp32_index.json`
   - Tools → Board → Boards Manager → Search "ESP32" → Install

3. **Required Libraries** (Arduino IDE):
   ```
   - WiFi (built-in)
   - HTTPClient (built-in)
   - ArduinoJson (v6.21.0+)
   - DHT sensor library (v1.4.4+)
   - BH1750 (v1.3.0+)
   ```

### Installation (SecureESP32)

1. **Open firmware:**
   ```bash
   cd FirmWare/SecureESP32
   # Open SecureESP32.ino in Arduino IDE
   ```

2. **Configure WiFi and API:**

   Edit `config.h`:
   ```cpp
   // WiFi Configuration
   const char* WIFI_SSID = "YourWiFiNetwork";
   const char* WIFI_PASSWORD = "YourWiFiPassword";

   // API Configuration
   const char* API_BASE_URL = "https://your-api-domain.com/api";
   const char* API_REGISTER_ENDPOINT = "/auth/device/register";
   const char* API_LOGIN_ENDPOINT = "/auth/device/login";
   const char* API_TELEMETRY_ENDPOINT = "/telemetry";

   // Device Configuration
   const char* DEVICE_NAME = "SmartGarden-ESP32-01";
   const int PLANT_ID = 1;  // Set after registering device

   // Sensor Intervals
   const unsigned long READING_INTERVAL = 60000;  // 60 seconds
   const unsigned long DEEP_SLEEP_DURATION = 300;  // 5 minutes (battery mode)
   ```

3. **Select Board:**
   - Tools → Board → ESP32 Arduino → ESP32 Dev Module
   - Tools → Upload Speed → 115200
   - Tools → CPU Frequency → 240MHz
   - Tools → Flash Size → 4MB
   - Tools → Partition Scheme → Default 4MB with spiffs

4. **Connect ESP32:**
   - Connect via USB cable
   - Tools → Port → Select COM port

5. **Upload Firmware:**
   - Click "Upload" button
   - Wait for "Done uploading" message

6. **Monitor Serial Output:**
   - Tools → Serial Monitor
   - Set baud rate to 115200
   - Watch for connection status and sensor readings

### First Boot Configuration

1. **Device Registration:**
   ```
   [INFO] WiFi connected: 192.168.1.100
   [INFO] Registering device...
   [INFO] Device registered successfully
   [INFO] Device Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   [INFO] Please approve device in web app
   ```

2. **Approve Device in Web App:**
   - Open SmartGarden web app
   - Login to your account
   - Click "Approve" on pending device banner
   - Return to Serial Monitor

3. **First Telemetry:**
   ```
   [INFO] Device approved!
   [INFO] Sending telemetry...
   [INFO] Soil Moisture: 45.2%
   [INFO] Water Level: 75.0%
   [INFO] Air Temp: 22.5°C
   [INFO] Humidity: 65.0%
   [INFO] Telemetry sent successfully
   [INFO] Command: NONE
   ```

## Sensor Calibration

### Soil Moisture Sensor

1. **Dry Calibration:**
   ```cpp
   // Place sensor in air (dry)
   int dryValue = analogRead(SOIL_MOISTURE_PIN);  // Note this value
   Serial.printf("Dry value: %d\n", dryValue);
   ```

2. **Wet Calibration:**
   ```cpp
   // Place sensor in water (wet)
   int wetValue = analogRead(SOIL_MOISTURE_PIN);  // Note this value
   Serial.printf("Wet value: %d\n", wetValue);
   ```

3. **Update Constants:**
   ```cpp
   #define SOIL_DRY_VALUE   3200  // Your dry value
   #define SOIL_WET_VALUE   1200  // Your wet value

   float getSoilMoisture() {
     int rawValue = analogRead(SOIL_MOISTURE_PIN);
     float percentage = map(rawValue, SOIL_WET_VALUE, SOIL_DRY_VALUE, 100, 0);
     return constrain(percentage, 0, 100);
   }
   ```

### Water Level Sensor

1. **Empty Tank:**
   ```cpp
   int emptyValue = analogRead(WATER_LEVEL_PIN);  // Tank empty
   ```

2. **Full Tank:**
   ```cpp
   int fullValue = analogRead(WATER_LEVEL_PIN);   // Tank full
   ```

3. **Update Mapping:**
   ```cpp
   #define WATER_EMPTY_VALUE  500
   #define WATER_FULL_VALUE   3500

   float getWaterLevel() {
     int rawValue = analogRead(WATER_LEVEL_PIN);
     float percentage = map(rawValue, WATER_EMPTY_VALUE, WATER_FULL_VALUE, 0, 100);
     return constrain(percentage, 0, 100);
   }
   ```

## Firmware Features

### Automatic Watering Logic

The ESP32 firmware works with the backend to determine when to water:

```cpp
void handleWateringCommand(String command, int duration) {
  if (command == "WATER" && duration > 0) {
    Serial.printf("Watering for %d seconds\n", duration);

    digitalWrite(PUMP_PIN, HIGH);  // Turn pump ON
    delay(duration * 1000);        // Wait for duration
    digitalWrite(PUMP_PIN, LOW);   // Turn pump OFF

    Serial.println("Watering complete");
  }
}
```

**Backend determines watering based on:**
- Current soil moisture < threshold
- Water tank level > 5%
- Time since last watering

### Deep Sleep Mode (Battery Operation)

```cpp
void enterDeepSleep() {
  Serial.println("Entering deep sleep for 5 minutes");

  // Configure wake-up
  esp_sleep_enable_timer_wakeup(DEEP_SLEEP_DURATION * 1000000);

  // Enter deep sleep
  esp_deep_sleep_start();
}

void loop() {
  readSensors();
  sendTelemetry();

  #ifdef BATTERY_MODE
    enterDeepSleep();  // Sleep to save battery
  #else
    delay(60000);      // Normal mode: 60 second delay
  #endif
}
```

**Power Consumption:**
- Active mode: ~100-150mA
- Deep sleep: ~10-20μA
- Battery life: 2000mAh battery = 30+ days (5-minute intervals)

### Error Recovery

```cpp
// Watchdog Timer
#include <esp_task_wdt.h>

void setup() {
  // Enable watchdog (30 seconds)
  esp_task_wdt_init(30, true);
  esp_task_wdt_add(NULL);
}

void loop() {
  // Reset watchdog
  esp_task_wdt_reset();

  // Your code here
}
```

**Automatic Recovery:**
- WiFi disconnection → Auto-reconnect (exponential backoff)
- API timeout → Retry up to 3 times
- Watchdog timeout → ESP32 reboot
- Token expiration → Auto-refresh

### OTA Updates (Over-The-Air)

```cpp
#include <ArduinoOTA.h>

void setupOTA() {
  ArduinoOTA.setHostname("SmartGarden-ESP32");
  ArduinoOTA.setPassword("your-ota-password");

  ArduinoOTA.onStart([]() {
    Serial.println("OTA Update Started");
  });

  ArduinoOTA.onEnd([]() {
    Serial.println("OTA Update Complete");
  });

  ArduinoOTA.onError([](ota_error_t error) {
    Serial.printf("OTA Error: %u\n", error);
  });

  ArduinoOTA.begin();
}

void loop() {
  ArduinoOTA.handle();  // Handle OTA requests
  // Rest of your code
}
```

**Update via Arduino IDE:**
- Tools → Port → Select "SmartGarden-ESP32 at 192.168.1.100"
- Click "Upload"
- Firmware updates wirelessly

## Communication Protocol

### Device Registration (First Boot)

```
POST /api/auth/device/register
Content-Type: application/json

{
  "macAddress": "AA:BB:CC:DD:EE:FF",
  "deviceName": "SmartGarden-ESP32-01",
  "firmwareVersion": "2.0.0"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "deviceId": 1,
  "isApproved": false
}
```

### Device Login (Get Fresh Token)

```
POST /api/auth/device/login
Content-Type: application/json

{
  "macAddress": "AA:BB:CC:DD:EE:FF"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 86400,
  "deviceId": 1,
  "isApproved": true
}
```

### Send Telemetry

```
POST /api/telemetry
Authorization: Bearer {device_token}
Content-Type: application/json

{
  "plantId": 1,
  "soilMoisture": 45.5,
  "waterLevel": 75.0,
  "airTemp": 22.5,
  "airHumidity": 65.0,
  "lightLevel": 800,
  "airQuality": 95.0
}

Response:
{
  "command": "WATER",  // or "NONE"
  "duration": 3000     // milliseconds
}
```

## Troubleshooting

### WiFi Connection Issues

**Problem:** ESP32 cannot connect to WiFi

**Solutions:**
1. Check SSID and password in `config.h`
2. Ensure WiFi is 2.4GHz (ESP32 doesn't support 5GHz)
3. Check WiFi signal strength (try moving closer to router)
4. Verify router allows new device connections
5. Try forgetting and re-adding network:
   ```cpp
   WiFi.disconnect(true);  // Erase saved credentials
   WiFi.begin(ssid, password);
   ```

### Sensor Reading Issues

**Problem:** Sensor readings are 0 or incorrect

**Solutions:**
1. Check sensor wiring and connections
2. Verify pin definitions in code match hardware
3. Re-run sensor calibration
4. Check sensor power supply (3.3V or 5V)
5. Test sensor with multimeter
6. Replace faulty sensor

### API Communication Errors

**Problem:** "HTTP Error 401" or "Request timeout"

**Solutions:**
1. Verify API URL is correct in `config.h`
2. Check device is approved in web app
3. Ensure backend API is running
4. Verify firewall allows HTTPS connections
5. Check JWT token hasn't expired (auto-refresh should handle this)
6. Inspect Serial Monitor for detailed error messages

### Watering Not Working

**Problem:** Pump doesn't activate when commanded

**Solutions:**
1. Check pump wiring to GPIO 26
2. Verify pump power supply (separate from ESP32)
3. Test relay module manually
4. Check pump is not clogged
5. Verify water source has water
6. Check Serial Monitor for watering commands

### Device Not Registering

**Problem:** Device registration fails

**Solutions:**
1. Ensure backend API is accessible
2. Check API endpoints in `config.h`
3. Verify MAC address is unique
4. Check backend database connection
5. Review backend logs for errors

## Advanced Features

### Multi-Plant Support

Configure multiple plants on one ESP32:

```cpp
const int NUM_PLANTS = 3;

struct PlantConfig {
  int plantId;
  int soilPin;
  int pumpPin;
};

PlantConfig plants[NUM_PLANTS] = {
  {1, 34, 26},
  {2, 35, 25},
  {3, 32, 33}
};

void readAndSendAll() {
  for (int i = 0; i < NUM_PLANTS; i++) {
    float moisture = readMoisture(plants[i].soilPin);
    sendTelemetry(plants[i].plantId, moisture);
  }
}
```

### MQTT Integration (Future)

```cpp
#include <PubSubClient.h>

// Subscribe to watering commands
client.subscribe("smartgarden/plant/1/command");

// Publish sensor readings
client.publish("smartgarden/plant/1/moisture", "45.5");
```

### Local Control (Without Backend)

```cpp
void localWateringLogic() {
  float moisture = getSoilMoisture();

  if (moisture < 30.0 && millis() - lastWatered > 3600000) {
    waterPlant(3000);  // Water for 3 seconds
    lastWatered = millis();
  }
}
```

## Security Best Practices

1. **Change default passwords** in `config.h`
2. **Use HTTPS** for all API communication
3. **Store tokens** in SPIFFS or EEPROM (persistent storage)
4. **Enable OTA password** protection
5. **Disable serial debug** in production
6. **Implement rate limiting** to prevent API abuse
7. **Use certificate pinning** for HTTPS (optional)

## Performance Optimization

1. **Reduce reading frequency** when battery powered
2. **Batch sensor readings** before sending
3. **Use deep sleep** between readings
4. **Disable unused sensors** to save power
5. **Optimize WiFi** (reduce reconnect attempts)
6. **Use local caching** for failover

## File Structure

```
FirmWare/
├── SecureESP32/
│   ├── SecureESP32.ino      # Main firmware file
│   ├── config.h             # Configuration
│   ├── sensors.h            # Sensor functions
│   ├── networking.h         # WiFi and HTTP
│   ├── watering.h           # Pump control
│   └── README.md            # Detailed docs
├── EspCode/
│   └── basic_esp32.ino      # Basic firmware
├── ModulWifiCod/
│   └── wifi_config.ino      # WiFi setup
├── ArduinoCOd/
│   └── arduino_basic.ino    # Arduino version
└── README.md                # This file
```

## Related Documentation

- [Main Project Documentation](../PROJECT_DOCUMENTATION.md)
- [Backend API README](../MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/README.md)
- [Frontend README](../ReactNativeApp/SmartGardenApp/README.md)
- [API Reference](../API_REFERENCE.md)

## Support

For firmware issues:
- Check Serial Monitor output
- Review hardware connections
- Verify sensor calibration
- Test with basic example code
- Check ESP32 is not damaged

## Contributing

Contributions welcome! Areas for improvement:
- Add MQTT support
- Implement BLE configuration
- Add more sensor types
- Improve power efficiency
- Create mobile app for local control

---

**Version:** 2.0
**Last Updated:** November 2025
**Compatible Hardware:** ESP32, ESP32-WROOM-32, ESP32-DevKitC
**License:** MIT
