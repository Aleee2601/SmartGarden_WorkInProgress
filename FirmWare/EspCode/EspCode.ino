#include <WiFi.h>
#include <HTTPClient.h>
#include <Wire.h>
#include <SHT2x.h>
#include <BH1750.h>

// ====== CONFIGURARE ======
const char* ssid = "Alexandra";
const char* password = "26012005";
const char* backendUrl = "http://172.20.10.3:5000/api"; // adresa API backend

// Pinuri hardware
#define SOIL_MOISTURE_PIN 34
#define PUMP_PIN 26
#define MQ135_PIN 35
#define TRIG_PIN 32
#define ECHO_PIN 33

// Senzori
SHT2x sht;
BH1750 lightMeter;

// Variabile
unsigned long lastReadTime = 0;
unsigned long readInterval = 900000; // 15 minute default
bool pumpState = false;
bool calibrationMode = false; // mod calibrare

void setup() {
  Serial.begin(115200);
  Wire.begin();

  // Pornire senzori
  sht.begin();
  lightMeter.begin(BH1750::CONTINUOUS_HIGH_RES_MODE);

  pinMode(PUMP_PIN, OUTPUT);
  digitalWrite(PUMP_PIN, LOW);
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  // Conectare WiFi
  WiFi.begin(ssid, password);
  Serial.print("Conectare la WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConectat la retea.");
}

void loop() {
  unsigned long currentMillis = millis();

  // verificare mod calibrare
  checkCalibrationMode();

  // Citire si trimitere date la interval
  if (currentMillis - lastReadTime >= readInterval) {
    lastReadTime = currentMillis;
    sendSensorData();
  }

  // Verificare comenzi backend pentru udare
  checkWaterCommand();

  delay(200);
}

void sendSensorData() {
  if (WiFi.status() != WL_CONNECTED) return;

  // Citiri senzori
  float temperature = sht.getTemperature();
  float humidity = sht.getHumidity();
  int soilMoisture = analogRead(SOIL_MOISTURE_PIN);
  float lightLevel = lightMeter.readLightLevel();
  int airQuality = analogRead(MQ135_PIN);
  float distance = readUltrasonic();

  // Creare payload JSON
  String json = "{";
  json += "\"temperature\":" + String(temperature, 2) + ",";
  json += "\"humidity\":" + String(humidity, 2) + ",";
  json += "\"soilMoisture\":" + String(soilMoisture) + ",";
  json += "\"lightLevel\":" + String(lightLevel, 2) + ",";
  json += "\"airQuality\":" + String(airQuality) + ",";
  json += "\"waterLevel\":" + String(distance, 2);
  json += "}";

  // Trimite datele la backend
  HTTPClient http;
  http.begin(String(backendUrl) + "/plants/1/sensor/readings"); // ID planta hardcodat pt MVP
  http.addHeader("Content-Type", "application/json");
  int httpResponseCode = http.POST(json);

  Serial.printf("POST /sensor/readings => %d\n", httpResponseCode);
  http.end();
}

void checkWaterCommand() {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  http.begin(String(backendUrl) + "/plants/1/watering"); // ID planta hardcodat pt MVP
  int httpResponseCode = http.GET();

  if (httpResponseCode == 200) {
    String payload = http.getString();
    if (payload.indexOf("WATER ON") >= 0) {
      digitalWrite(PUMP_PIN, HIGH);
      pumpState = true;
      Serial.println("Pompa pornita");
    } 
    else if (payload.indexOf("WATER OFF") >= 0) {
      digitalWrite(PUMP_PIN, LOW);
      pumpState = false;
      Serial.println("Pompa oprita");
    }
  }

  http.end();
}

void checkCalibrationMode() {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  http.begin(String(backendUrl) + "/plants/1/calibration"); // Endpoint backend pentru calibrare
  int httpResponseCode = http.GET();

  if (httpResponseCode == 200) {
    String payload = http.getString();
    if (payload.indexOf("ON") >= 0) {
      if (!calibrationMode) {
        calibrationMode = true;
        readInterval = 1000; // 1 sec
        Serial.println("Calibration mode ACTIVAT - citire senzori la 1 secunda");
      }
    } else {
      if (calibrationMode) {
        calibrationMode = false;
        readInterval = 900000; // 15 min
        Serial.println("Calibration mode DEZACTIVAT - citire senzori la 15 minute");
      }
    }
  }

  http.end();
}

float readUltrasonic() {
  digitalWrite(TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);

  long duration = pulseIn(ECHO_PIN, HIGH);
  float distance = duration * 0.034 / 2;
  return distance; // cm
}
