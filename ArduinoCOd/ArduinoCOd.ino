#include <Wire.h>
#include <SHT2x.h>

SHT21 sht;
#define SOIL_MOISTURE_PIN A0
#define PUMP_PIN 8

bool pumpState = false;  // Controlled by ESP

void setup() {
  Serial.begin(9600);     // Communication with ESP-01
  sht.begin();            // Start SHT21
  pinMode(PUMP_PIN, OUTPUT);
  digitalWrite(PUMP_PIN, LOW);
}

void loop() {
  //  Read sensors
  int soilValue = analogRead(SOIL_MOISTURE_PIN);
  float temperature = sht.getTemperature();
  float humidity = sht.getHumidity();

  // Send sensor data to ESP-01 in JSON format
  Serial.print("{\"temperature\":");
  Serial.print(temperature);
  Serial.print(",\"humidity\":");
  Serial.print(humidity);
  Serial.print(",\"soil\":");
  Serial.print(soilValue);
  Serial.println("}");

  // Check if ESP-01 sent a command
  if (Serial.available()) {
    String cmd = Serial.readStringUntil('\n');
    cmd.trim();
    
    if (cmd == "WATER ON") {
      pumpState = true;
    } else if (cmd == "WATER OFF") {
      pumpState = false;
    }
  }

  // Set pump based on received command
  digitalWrite(PUMP_PIN, pumpState ? HIGH : LOW);

  delay(5000);  // Delay between readings (adjustable)
}
