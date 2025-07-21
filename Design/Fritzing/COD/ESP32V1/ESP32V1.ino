#include <WiFi.h>
#include <WebServer.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_SHT31.h>
#include <BH1750.h>

// Wifi credentials
const char* ssid = "Alexandra";
const char* password = "26012005";

// Pini
#define SOIL_MOISTURE_PIN 34   // ADC1
#define PUMP_PIN 18            // Digital pin pentru MOSFET
#define MQ2_PIN 35             // ADC1
#define TRIG_PIN 13            // HC-SR04
#define ECHO_PIN 12

// Obiecte senzori
Adafruit_SHT31 sht = Adafruit_SHT31();
BH1750 lightMeter;
WebServer server(80);

// Date senzori
float temperature = 0;
float humidity = 0;
int soilMoisture = 0;
float lightLevel = 0;
int gasLevel = 0;
int waterLevel = 0;

bool pumpState = false;

void setup() {
  Serial.begin(115200);
  Serial.println("Pornire SmartGarden ESP32..."); // DEBUG

  // Inițializări pini
  pinMode(PUMP_PIN, OUTPUT);
  digitalWrite(PUMP_PIN, LOW);
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  // Inițializare I2C și senzori
  Wire.begin(21, 22); // I2C pentru ESP32

  if (!sht.begin(0x44)) {
    Serial.println("Eroare la senzorul SHT31!"); // DEBUG
  } else {
    Serial.println("Senzor SHT31 detectat cu succes."); // DEBUG
  }

  if (!lightMeter.begin(BH1750::CONTINUOUS_HIGH_RES_MODE)) {
    Serial.println("Eroare la senzorul BH1750!"); // DEBUG
  } else {
    Serial.println("Senzor BH1750 detectat cu succes."); // DEBUG
  }

  // Pornire WiFi AP
  WiFi.softAP(ssid, password);
  Serial.println("WiFi AP activat: " + String(ssid)); // DEBUG
  Serial.print("IP local: "); Serial.println(WiFi.softAPIP()); // DEBUG

  // Server HTTP
  server.on("/data", HTTP_GET, handleSensorData);
  server.on("/water", HTTP_POST, handleWaterCommand);
  server.begin();
  Serial.println("Server HTTP pornit pe portul 80"); // DEBUG
}

void loop() {
  server.handleClient();
  readSensors();
  controlPump();
  delay(5000); // Refresh la 5 secunde
}

void handleSensorData() {
  String json = "{";
  json += "\"temperature\":" + String(temperature, 2) + ",";
  json += "\"humidity\":" + String(humidity, 2) + ",";
  json += "\"soilMoisture\":" + String(soilMoisture) + ",";
  json += "\"light\":" + String(lightLevel, 2) + ",";
  json += "\"gas\":" + String(gasLevel) + ",";
  json += "\"waterLevel\":" + String(waterLevel);
  json += "}";
  server.send(200, "application/json", json);
  Serial.println("[HTTP] GET /data trimis"); // DEBUG
}

void handleWaterCommand() {
  String cmd = server.arg("plain");
  cmd.trim();
  Serial.println("[HTTP] POST /water: " + cmd); // DEBUG

  if (cmd == "WATER ON") {
    pumpState = true;
    server.send(200, "text/plain", "Pump turned ON");
  } else if (cmd == "WATER OFF") {
    pumpState = false;
    server.send(200, "text/plain", "Pump turned OFF");
  } else {
    server.send(400, "text/plain", "Invalid command");
  }
}

void readSensors() {
  temperature = sht.readTemperature();
  humidity = sht.readHumidity();
  soilMoisture = analogRead(SOIL_MOISTURE_PIN);
  lightLevel = lightMeter.readLightLevel();
  gasLevel = analogRead(MQ2_PIN);
  waterLevel = readUltrasonicDistance();

  // DEBUG: Afișare valori
  Serial.println("---- Citiri senzori ----");
  Serial.print("Temp: "); Serial.print(temperature); Serial.println(" °C");
  Serial.print("Umiditate aer: "); Serial.print(humidity); Serial.println(" %");
  Serial.print("Umiditate sol: "); Serial.println(soilMoisture);
  Serial.print("Luminozitate: "); Serial.print(lightLevel); Serial.println(" lx");
  Serial.print("Gaz MQ2: "); Serial.println(gasLevel);
  Serial.print("Distanță (apă): "); Serial.print(waterLevel); Serial.println(" cm");
  Serial.println("------------------------");
}

void controlPump() {
  digitalWrite(PUMP_PIN, pumpState ? HIGH : LOW);
  Serial.println(pumpState ? "Pompa: PORNITĂ" : "Pompa: OPRITĂ"); // DEBUG
}

int readUltrasonicDistance() {
  digitalWrite(TRIG_PIN, LOW); delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH); delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);
  long duration = pulseIn(ECHO_PIN, HIGH);
  int distance = duration * 0.034 / 2; // cm
  return distance;
}
