/**
 * SmartGarden Secure ESP32 Firmware
 *
 * Features:
 * - HTTPS/TLS communication with backend
 * - Device JWT authentication
 * - HMAC-SHA256 message signing
 * - Secure credential storage in EEPROM
 * - Automatic token refresh
 * - Rate limiting awareness
 * - WiFi auto-reconnect
 *
 * Hardware:
 * - ESP-WROOM-32
 * - SHT21 (Temperature & Humidity)
 * - BH1750 (Light sensor)
 * - Capacitive soil moisture sensor (analog)
 * - MQ-135 (Air quality)
 * - HC-SR04 (Ultrasonic water level)
 * - IRF520 MOSFET (Pump control)
 */

#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <SHT2x.h>
#include <BH1750.h>
#include <EEPROM.h>
#include <mbedtls/md.h>

// ====== CONFIGURATION ======
#define WIFI_SSID "YOUR_WIFI_SSID"
#define WIFI_PASSWORD "YOUR_WIFI_PASSWORD"
#define API_BASE_URL "https://your-api-domain.com/api"  // IMPORTANT: Use HTTPS!
#define FIRMWARE_VERSION "1.0.0"
#define MODEL_NAME "ESP32-SmartGarden-v1"

// Pin definitions
#define SOIL_MOISTURE_PIN 34
#define PUMP_PIN 26
#define MQ135_PIN 35
#define TRIG_PIN 32
#define ECHO_PIN 33

// Timing constants
#define HEARTBEAT_INTERVAL 60000    // 1 minute
#define DEFAULT_READ_INTERVAL 900000 // 15 minutes
#define CALIBRATION_INTERVAL 1000    // 1 second during calibration
#define TOKEN_REFRESH_BEFORE_EXPIRY 3600000 // Refresh 1 hour before expiry
#define MAX_RETRY_ATTEMPTS 3
#define RETRY_DELAY_MS 2000

// EEPROM addresses for secure storage
#define EEPROM_SIZE 512
#define ADDR_DEVICE_ID 0
#define ADDR_DEVICE_TOKEN 50
#define ADDR_API_KEY 200
#define ADDR_REFRESH_TOKEN 300
#define ADDR_PLANT_ID 450
#define ADDR_IS_REGISTERED 460

// ====== GLOBAL VARIABLES ======
SHT2x sht;
BH1750 lightMeter;
WiFiClientSecure secureClient;

// Device credentials (loaded from EEPROM)
String deviceId = "";
String deviceToken = "";
String apiKey = "";
String refreshToken = "";
int plantId = 0;
bool isRegistered = false;

// State variables
unsigned long lastReadTime = 0;
unsigned long lastHeartbeatTime = 0;
unsigned long readInterval = DEFAULT_READ_INTERVAL;
unsigned long tokenExpiry = 0;
bool pumpState = false;
bool calibrationMode = false;
bool isApproved = false;

// WiFi reconnection
unsigned long lastWiFiCheck = 0;
#define WIFI_CHECK_INTERVAL 30000

// Root CA Certificate (Let's Encrypt)
// Replace with your actual certificate
const char* rootCACertificate = \
"-----BEGIN CERTIFICATE-----\n" \
"MIIFFjCCAv6gAwIBAgIRAJErCErPDBinU/bWLiWnX1owDQYJKoZIhvcNAQELBQAw\n" \
"TzELMAkGA1UEBhMCVVMxKTAnBgNVBAoTIEludGVybmV0IFNlY3VyaXR5IFJlc2Vh\n" \
"cmNoIEdyb3VwMRUwEwYDVQQDEwxJU1JHIFJvb3QgWDEwHhcNMjAwOTA0MDAwMDAw\n" \
"WhcNMjUwOTE1MTYwMDAwWjAyMQswCQYDVQQGEwJVUzEWMBQGA1UEChMNTGV0J3Mg\n" \
"RW5jcnlwdDELMAkGA1UEAxMCUjMwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEK\n" \
"AoIBAQC7AhUozPaglNMPEuyNVZLD+ILxmaZ6QoinXSaqtSu5xUyxr45r+XXIo9cP\n" \
"R5QUVTVXjJ6oojkZ9YI8QqlObvU7wy7bjcCwXPNZOOftz2nwWgsbvsCUJCWH+jdx\n" \
"sxPnHKzhm+/b5DtFUkWWqcFTzjTIUu61ru2P3mBw4qVUq7ZtDpelQDRrK9O8Zutm\n" \
"NHz6a4uPVymZ+DAXXbpyb/uBxa3Shlg9F8fnCbvxK/eG3MHacV3URuPMrSXBiLxg\n" \
"Z3Vms/EY96Jc5lP/Ooi2R6X/ExjqmAl3P51T+c8B5fWmcBcUr2Ok/5mzk53cU6cG\n" \
"/kiFHaFpriV1uxPMUgP17VGhi9sVAgMBAAGjggEIMIIBBDAOBgNVHQ8BAf8EBAMC\n" \
"AYYwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMBMBIGA1UdEwEB/wQIMAYB\n" \
"Af8CAQAwHQYDVR0OBBYEFBQusxe3WFbLrlAJQOYfr52LFMLGMB8GA1UdIwQYMBaA\n" \
"FHm0WeZ7tuXkAXOACIjIGlj26ZtuMDIGCCsGAQUFBwEBBCYwJDAiBggrBgEFBQcw\n" \
"AoYWaHR0cDovL3gxLmkubGVuY3Iub3JnLzAnBgNVHR8EIDAeMBygGqAYhhZodHRw\n" \
"Oi8veDEuYy5sZW5jci5vcmcvMCIGA1UdIAQbMBkwCAYGZ4EMAQIBMA0GCysGAQQB\n" \
"gt8TAQEBMA0GCSqGSIb3DQEBCwUAA4ICAQCFyk5HPqP3hUSFvNVneLKYY611TR6W\n" \
"PTNlclQtgaDqw+34IL9fzLdwALduO/ZelN7kIJ+m74uyA+eitRY8kc607TkC53wl\n" \
"ikfmZW4/RvTZ8M6UK+5UzhK8jCdLuMGYL6KvzXGRSgi3yLgjewQtCPkIVz6D2QQz\n" \
"CkcheAmCJ8MqyJu5zlzyZMjAvnnAT45tRAxekrsu94sQ4egdRCnbWSDtY7kh+BIm\n" \
"lJNXoB1lBMEKIq4QDUOXoRgffuDghje1WrG9ML+Hbisq/yFOGwXD9RiX8F6sw6W4\n" \
"avAuvDszue5L3sz85K+EC4Y/wFVDNvZo4TYXao6Z0f+lQKc0t8DQYzk1OXVu8rp2\n" \
"yJMC6alLbBfODALZvYH7n7do1AZls4I9d1P4jnkDrQoxB3UqQ9hVl3LEKQ73xF1O\n" \
"yK5GhDDX8oVfGKF5u+decIsH4YaTw7mP3GFxJSqv3+0lUFJoi5Lc5da149p90Ids\n" \
"hCExroL1+7mryIkXPeFM5TgO9r0rvZaBFOvV2z0gp35Z0+L4WPlbuEjN/lxPFin+\n" \
"HlUjr8gRsI3qfJOQFy/9rKIJR0Y/8Omwt/8oTWgy1mdeHmmjk7j1nYsvC9JSQ6Zv\n" \
"MldlTTKB3zhThV1+XWYp6rjd5JW1zbVWEkLNxE7GJThEUG3szgBVGP7pSWTUTsqX\n" \
"nLRbwHOoq7hHwg==\n" \
"-----END CERTIFICATE-----\n";

// ====== FUNCTION DECLARATIONS ======
void setup();
void loop();
void setupWiFi();
void ensureWiFiConnected();
void setupSensors();
void setupSecureClient();
bool loadCredentialsFromEEPROM();
void saveCredentialsToEEPROM();
bool registerDevice();
bool refreshDeviceToken();
void sendHeartbeat();
void sendSensorData();
void checkForCommands();
void checkCalibrationMode();
float readSoilMoisture();
float readAirTemperature();
float readAirHumidity();
float readLightLevel();
float readAirQuality();
float readWaterLevel();
String signPayload(String payload);
String hmacSHA256(String data, String key);
bool makeSecureRequest(String endpoint, String method, String payload, String& response);
void controlPump(bool state);
void blinkLED(int times);

// ====== SETUP ======
void setup() {
    Serial.begin(115200);
    Serial.println("\n\n===== SmartGarden Secure ESP32 =====");
    Serial.println("Firmware Version: " + String(FIRMWARE_VERSION));

    // Initialize EEPROM
    EEPROM.begin(EEPROM_SIZE);

    // Initialize pins
    pinMode(PUMP_PIN, OUTPUT);
    digitalWrite(PUMP_PIN, LOW);
    pinMode(LED_BUILTIN, OUTPUT);
    pinMode(TRIG_PIN, OUTPUT);
    pinMode(ECHO_PIN, INPUT);

    // Setup WiFi
    setupWiFi();

    // Setup sensors
    setupSensors();

    // Setup secure HTTPS client
    setupSecureClient();

    // Load credentials from EEPROM
    bool hasCredentials = loadCredentialsFromEEPROM();

    if (!hasCredentials || !isRegistered) {
        Serial.println("Device not registered. Starting registration...");
        if (registerDevice()) {
            Serial.println("Device registered successfully!");
            saveCredentialsToEEPROM();
        } else {
            Serial.println("Device registration failed. Will retry on next boot.");
        }
    } else {
        Serial.println("Device already registered: " + deviceId);
        Serial.println("Checking token validity...");

        // Check if token needs refresh
        if (millis() > tokenExpiry - TOKEN_REFRESH_BEFORE_EXPIRY) {
            Serial.println("Token expiring soon, refreshing...");
            refreshDeviceToken();
        }
    }

    Serial.println("Setup complete. Starting main loop...");
    blinkLED(3);
}

// ====== MAIN LOOP ======
void loop() {
    unsigned long currentMillis = millis();

    // Ensure WiFi is connected
    if (currentMillis - lastWiFiCheck >= WIFI_CHECK_INTERVAL) {
        ensureWiFiConnected();
        lastWiFiCheck = currentMillis;
    }

    // Only proceed if device is registered and approved
    if (!isRegistered) {
        delay(10000); // Wait 10 seconds before retrying
        if (registerDevice()) {
            saveCredentialsToEEPROM();
        }
        return;
    }

    // Send heartbeat
    if (currentMillis - lastHeartbeatTime >= HEARTBEAT_INTERVAL) {
        sendHeartbeat();
        lastHeartbeatTime = currentMillis;
    }

    // Check calibration mode
    checkCalibrationMode();

    // Send sensor data at interval
    if (currentMillis - lastReadTime >= readInterval) {
        sendSensorData();
        lastReadTime = currentMillis;
    }

    // Check for commands from backend
    checkForCommands();

    // Check token expiry
    if (currentMillis > tokenExpiry - TOKEN_REFRESH_BEFORE_EXPIRY) {
        refreshDeviceToken();
    }

    delay(1000);
}

// ====== WiFi FUNCTIONS ======
void setupWiFi() {
    Serial.println("Connecting to WiFi...");
    WiFi.mode(WIFI_STA);
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

    int attempts = 0;
    while (WiFi.status() != WL_CONNECTED && attempts < 20) {
        delay(500);
        Serial.print(".");
        attempts++;
    }

    if (WiFi.status() == WL_CONNECTED) {
        Serial.println("\nWiFi connected!");
        Serial.println("IP address: " + WiFi.localIP().toString());
        Serial.println("Signal strength (RSSI): " + String(WiFi.RSSI()) + " dBm");
    } else {
        Serial.println("\nWiFi connection failed!");
    }
}

void ensureWiFiConnected() {
    if (WiFi.status() != WL_CONNECTED) {
        Serial.println("WiFi disconnected. Reconnecting...");
        setupWiFi();
    }
}

// ====== SENSOR SETUP ======
void setupSensors() {
    Wire.begin();
    sht.begin();
    lightMeter.begin(BH1750::CONTINUOUS_HIGH_RES_MODE);
    Serial.println("Sensors initialized");
}

void setupSecureClient() {
    secureClient.setCACert(rootCACertificate);
    Serial.println("Secure HTTPS client configured");
}

// ====== CREDENTIALS MANAGEMENT ======
bool loadCredentialsFromEEPROM() {
    // Check if device is registered
    isRegistered = EEPROM.read(ADDR_IS_REGISTERED) == 1;

    if (!isRegistered) {
        return false;
    }

    // Load device ID
    deviceId = "";
    for (int i = 0; i < 40; i++) {
        char c = EEPROM.read(ADDR_DEVICE_ID + i);
        if (c == '\0') break;
        deviceId += c;
    }

    // Load device token
    deviceToken = "";
    for (int i = 0; i < 150; i++) {
        char c = EEPROM.read(ADDR_DEVICE_TOKEN + i);
        if (c == '\0') break;
        deviceToken += c;
    }

    // Load API key
    apiKey = "";
    for (int i = 0; i < 100; i++) {
        char c = EEPROM.read(ADDR_API_KEY + i);
        if (c == '\0') break;
        apiKey += c;
    }

    // Load refresh token
    refreshToken = "";
    for (int i = 0; i < 150; i++) {
        char c = EEPROM.read(ADDR_REFRESH_TOKEN + i);
        if (c == '\0') break;
        refreshToken += c;
    }

    // Load plant ID
    plantId = EEPROM.read(ADDR_PLANT_ID) | (EEPROM.read(ADDR_PLANT_ID + 1) << 8);

    Serial.println("Credentials loaded from EEPROM");
    return true;
}

void saveCredentialsToEEPROM() {
    // Mark as registered
    EEPROM.write(ADDR_IS_REGISTERED, 1);

    // Save device ID
    for (int i = 0; i < deviceId.length() && i < 40; i++) {
        EEPROM.write(ADDR_DEVICE_ID + i, deviceId[i]);
    }
    EEPROM.write(ADDR_DEVICE_ID + deviceId.length(), '\0');

    // Save device token
    for (int i = 0; i < deviceToken.length() && i < 150; i++) {
        EEPROM.write(ADDR_DEVICE_TOKEN + i, deviceToken[i]);
    }
    EEPROM.write(ADDR_DEVICE_TOKEN + deviceToken.length(), '\0');

    // Save API key
    for (int i = 0; i < apiKey.length() && i < 100; i++) {
        EEPROM.write(ADDR_API_KEY + i, apiKey[i]);
    }
    EEPROM.write(ADDR_API_KEY + apiKey.length(), '\0');

    // Save refresh token
    for (int i = 0; i < refreshToken.length() && i < 150; i++) {
        EEPROM.write(ADDR_REFRESH_TOKEN + i, refreshToken[i]);
    }
    EEPROM.write(ADDR_REFRESH_TOKEN + refreshToken.length(), '\0');

    // Save plant ID
    EEPROM.write(ADDR_PLANT_ID, plantId & 0xFF);
    EEPROM.write(ADDR_PLANT_ID + 1, (plantId >> 8) & 0xFF);

    EEPROM.commit();
    Serial.println("Credentials saved to EEPROM");
}

// [CONTINUED IN NEXT MESSAGE DUE TO LENGTH]
