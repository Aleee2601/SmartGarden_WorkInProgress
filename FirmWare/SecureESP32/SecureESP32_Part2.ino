// ====== DEVICE REGISTRATION ======
bool registerDevice() {
    Serial.println("Registering device with backend...");

    String macAddress = WiFi.macAddress();
    macAddress.replace(":", "-");

    StaticJsonDocument<512> doc;
    doc["macAddress"] = macAddress;
    doc["model"] = MODEL_NAME;
    doc["firmwareVersion"] = FIRMWARE_VERSION;
    doc["serialNumber"] = ESP.getChipId();

    String payload;
    serializeJson(doc, payload);

    String response;
    if (!makeSecureRequest("/device-auth/register", "POST", payload, response)) {
        Serial.println("Registration request failed");
        return false;
    }

    // Parse response
    StaticJsonDocument<1024> responseDoc;
    DeserializationError error = deserializeJson(responseDoc, response);

    if (error) {
        Serial.println("Failed to parse registration response");
        return false;
    }

    // Extract credentials
    deviceId = responseDoc["deviceId"].as<String>();
    deviceToken = responseDoc["deviceToken"].as<String>();
    apiKey = responseDoc["apiKey"].as<String>();
    refreshToken = responseDoc["refreshToken"].as<String>();

    int expiresIn = responseDoc["expiresIn"];
    tokenExpiry = millis() + (expiresIn * 1000);

    isRegistered = true;
    isApproved = false; // Requires user approval

    Serial.println("Device registered successfully!");
    Serial.println("Device ID: " + deviceId);
    Serial.println("Waiting for user approval...");

    return true;
}

// ====== TOKEN REFRESH ======
bool refreshDeviceToken() {
    Serial.println("Refreshing device token...");

    StaticJsonDocument<256> doc;
    doc["deviceId"] = deviceId;
    doc["refreshToken"] = refreshToken;

    String payload;
    serializeJson(doc, payload);

    String response;
    if (!makeSecureRequest("/device-auth/refresh-token", "POST", payload, response)) {
        Serial.println("Token refresh failed");
        return false;
    }

    // Parse response
    StaticJsonDocument<512> responseDoc;
    DeserializationError error = deserializeJson(responseDoc, response);

    if (error) {
        Serial.println("Failed to parse refresh response");
        return false;
    }

    deviceToken = responseDoc["deviceToken"].as<String>();
    int expiresIn = responseDoc["expiresIn"];
    tokenExpiry = millis() + (expiresIn * 1000);

    saveCredentialsToEEPROM();

    Serial.println("Token refreshed successfully");
    return true;
}

// ====== HEARTBEAT ======
void sendHeartbeat() {
    if (!isRegistered) return;

    Serial.println("Sending heartbeat...");

    StaticJsonDocument<256> doc;
    doc["deviceId"] = deviceId;
    doc["batteryLevel"] = nullptr; // No battery in this version
    doc["signalStrength"] = WiFi.RSSI();
    doc["firmwareVersion"] = FIRMWARE_VERSION;
    doc["ipAddress"] = WiFi.localIP().toString();

    String payload;
    serializeJson(doc, payload);

    String response;
    if (makeSecureRequest("/device-auth/heartbeat", "POST", payload, response)) {
        Serial.println("Heartbeat sent successfully");
        // Check if approved
        StaticJsonDocument<128> responseDoc;
        deserializeJson(responseDoc, response);
        // Could add approval status check here
    } else {
        Serial.println("Heartbeat failed");
    }
}

// ====== SENSOR DATA ======
void sendSensorData() {
    if (!isRegistered || !isApproved) {
        Serial.println("Cannot send sensor data: Device not approved");
        return;
    }

    Serial.println("Reading sensors and sending data...");

    // Read all sensors
    float soilMoisture = readSoilMoisture();
    float airTemp = readAirTemperature();
    float airHumidity = readAirHumidity();
    float lightLevel = readLightLevel();
    float airQuality = readAirQuality();
    float waterLevel = readWaterLevel();

    // Create JSON payload
    StaticJsonDocument<512> doc;
    doc["deviceId"] = deviceId;
    if (plantId > 0) {
        doc["plantId"] = plantId;
    }
    doc["soilMoisture"] = soilMoisture;
    doc["airTemperature"] = airTemp;
    doc["airHumidity"] = airHumidity;
    doc["lightLevel"] = lightLevel;
    doc["airQuality"] = airQuality;
    doc["waterLevel"] = waterLevel;
    doc["timestamp"] = millis() / 1000; // Unix timestamp

    String payload;
    serializeJson(doc, payload);

    // Sign the payload
    String signature = signPayload(payload);
    doc["signature"] = signature;

    payload = "";
    serializeJson(doc, payload);

    // Send to backend
    String response;
    if (makeSecureRequest("/sensor", "POST", payload, response)) {
        Serial.println("Sensor data sent successfully");
    } else {
        Serial.println("Failed to send sensor data");
    }
}

// ====== COMMANDS ======
void checkForCommands() {
    if (!isRegistered || !isApproved) return;

    String response;
    String endpoint = "/watering/device/" + deviceId + "/commands";

    if (makeSecureRequest(endpoint, "GET", "", response)) {
        StaticJsonDocument<256> doc;
        DeserializationError error = deserializeJson(doc, response);

        if (!error) {
            bool waterNow = doc["waterNow"] | false;
            int duration = doc["duration"] | 0;

            if (waterNow && duration > 0 && !pumpState) {
                Serial.println("Watering command received: " + String(duration) + " seconds");
                controlPump(true);
                delay(duration * 1000);
                controlPump(false);
            }
        }
    }
}

void checkCalibrationMode() {
    if (!isRegistered || !isApproved) return;

    String response;
    String endpoint = "/plants/" + String(plantId) + "/calibration";

    if (makeSecureRequest(endpoint, "GET", "", response)) {
        StaticJsonDocument<128> doc;
        deserializeJson(doc, response);

        bool calibMode = doc["calibrationMode"] | false;

        if (calibMode != calibrationMode) {
            calibrationMode = calibMode;

            if (calibrationMode) {
                readInterval = CALIBRATION_INTERVAL;
                Serial.println("Calibration mode ACTIVATED - Reading every 1 second");
            } else {
                readInterval = DEFAULT_READ_INTERVAL;
                Serial.println("Calibration mode DEACTIVATED - Reading every 15 minutes");
            }
        }
    }
}

// ====== SENSOR READING FUNCTIONS ======
float readSoilMoisture() {
    int rawValue = analogRead(SOIL_MOISTURE_PIN);
    // Convert to percentage (calibrate these values for your sensor)
    float moisture = map(rawValue, 0, 4095, 0, 100);
    return constrain(moisture, 0, 100);
}

float readAirTemperature() {
    return sht.getTemperature();
}

float readAirHumidity() {
    return sht.getHumidity();
}

float readLightLevel() {
    return lightMeter.readLightLevel();
}

float readAirQuality() {
    int rawValue = analogRead(MQ135_PIN);
    // Convert to air quality index (simplified)
    return map(rawValue, 0, 4095, 0, 500);
}

float readWaterLevel() {
    digitalWrite(TRIG_PIN, LOW);
    delayMicroseconds(2);
    digitalWrite(TRIG_PIN, HIGH);
    delayMicroseconds(10);
    digitalWrite(TRIG_PIN, LOW);

    long duration = pulseIn(ECHO_PIN, HIGH, 30000); // 30ms timeout
    float distance = (duration * 0.034) / 2.0; // cm

    // Convert to percentage (assuming 20cm tank depth)
    float level = map(distance, 20, 0, 0, 100);
    return constrain(level, 0, 100);
}

// ====== SECURITY FUNCTIONS ======
String signPayload(String payload) {
    return hmacSHA256(payload, apiKey);
}

String hmacSHA256(String data, String key) {
    byte hmac[32];
    mbedtls_md_context_t ctx;
    mbedtls_md_type_t md_type = MBEDTLS_MD_SHA256;

    mbedtls_md_init(&ctx);
    mbedtls_md_setup(&ctx, mbedtls_md_info_from_type(md_type), 1);
    mbedtls_md_hmac_starts(&ctx, (const unsigned char*)key.c_str(), key.length());
    mbedtls_md_hmac_update(&ctx, (const unsigned char*)data.c_str(), data.length());
    mbedtls_md_hmac_finish(&ctx, hmac);
    mbedtls_md_free(&ctx);

    // Convert to hex string
    String signature = "";
    for (int i = 0; i < 32; i++) {
        if (hmac[i] < 0x10) signature += "0";
        signature += String(hmac[i], HEX);
    }

    return signature;
}

// ====== HTTPS REQUEST FUNCTION ======
bool makeSecureRequest(String endpoint, String method, String payload, String& response) {
    if (WiFi.status() != WL_CONNECTED) {
        Serial.println("WiFi not connected");
        return false;
    }

    HTTPClient https;
    String url = String(API_BASE_URL) + endpoint;

    Serial.println(method + " " + url);

    if (!https.begin(secureClient, url)) {
        Serial.println("HTTPS connection failed");
        return false;
    }

    // Add headers
    https.addHeader("Content-Type", "application/json");

    if (!deviceToken.isEmpty()) {
        https.addHeader("Authorization", "Bearer " + deviceToken);
    }

    if (!deviceId.isEmpty()) {
        https.addHeader("X-Device-ID", deviceId);
    }

    // Make request with retry
    int httpCode = -1;
    for (int attempt = 0; attempt < MAX_RETRY_ATTEMPTS; attempt++) {
        if (method == "POST") {
            httpCode = https.POST(payload);
        } else if (method == "GET") {
            httpCode = https.GET();
        } else if (method == "PUT") {
            httpCode = https.PUT(payload);
        }

        if (httpCode > 0) {
            break; // Success
        }

        Serial.println("Request failed, retrying... (" + String(attempt + 1) + "/" + String(MAX_RETRY_ATTEMPTS) + ")");
        delay(RETRY_DELAY_MS * (attempt + 1)); // Exponential backoff
    }

    if (httpCode > 0) {
        response = https.getString();
        Serial.println("HTTP Code: " + String(httpCode));

        if (httpCode == 401) {
            Serial.println("Unauthorized - Token may have expired");
            refreshDeviceToken();
        }

        https.end();
        return (httpCode >= 200 && httpCode < 300);
    } else {
        Serial.println("Request failed: " + https.errorToString(httpCode));
        https.end();
        return false;
    }
}

// ====== PUMP CONTROL ======
void controlPump(bool state) {
    pumpState = state;
    digitalWrite(PUMP_PIN, state ? HIGH : LOW);
    Serial.println("Pump: " + String(state ? "ON" : "OFF"));
}

// ====== LED BLINK ======
void blinkLED(int times) {
    for (int i = 0; i < times; i++) {
        digitalWrite(LED_BUILTIN, HIGH);
        delay(200);
        digitalWrite(LED_BUILTIN, LOW);
        delay(200);
    }
}
