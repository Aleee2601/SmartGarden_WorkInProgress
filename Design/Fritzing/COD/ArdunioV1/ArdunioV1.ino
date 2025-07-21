#include <Wire.h>
#include <LiquidCrystal.h>
#include <SHT2x.h>

#define SOIL_PIN A0
#define PUMP_PIN 8
#define LED_PIN 7

// LCD: RS, E, D4, D5, D6, D7
LiquidCrystal lcd(3, 4, 5, 6, 9, 10);
SHT21 sht;

bool pumpState = false;
String lastStatus = "Connecting...";
String lastMode = "AUTO";
String lastTime = "";

void setup() {
  pinMode(PUMP_PIN, OUTPUT);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(PUMP_PIN, LOW);
  digitalWrite(LED_PIN, LOW);

  Serial.begin(9600);
  Wire.begin();
  sht.begin();

  lcd.begin(16, 2);
  lcd.print("SmartGarden");
  delay(2000);
  lcd.clear();
}

void loop() {
  // 1. Citim senzorii
  int soilValue = analogRead(SOIL_PIN);
  float temperature = sht.getTemperature();
  float humidity = sht.getHumidity();

  // 2. Trimitem JSON către ESP
  Serial.print("{\"temperature\":");
  Serial.print(temperature);
  Serial.print(",\"humidity\":");
  Serial.print(humidity);
  Serial.print(",\"soil\":");
  Serial.print(soilValue);
  Serial.print(",\"watering\":");
  Serial.print(pumpState ? "true" : "false");
  Serial.println("}");

  // 3. Verificăm dacă ESP trimite ceva
  if (Serial.available()) {
    String cmd = Serial.readStringUntil('\n');
    cmd.trim();

    if (cmd == "WATER ON") {
      digitalWrite(PUMP_PIN, HIGH);
      blinkLed();
      pumpState = true;
    } else if (cmd == "WATER OFF") {
      digitalWrite(PUMP_PIN, LOW);
      digitalWrite(LED_PIN, LOW);
      pumpState = false;
    } else if (cmd.startsWith("STATUS:")) {
      lastStatus = cmd.substring(7);
    } else if (cmd.startsWith("MODE:")) {
      lastMode = cmd.substring(5);
    } else if (cmd.startsWith("TIME:")) {
      lastTime = cmd.substring(5);
    }
  }

  // 4. Afișăm pe LCD
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(lastStatus);  // ex: Connected
  lcd.setCursor(0, 1);
  lcd.print("Sol:");
  lcd.print(analogRead(SOIL_PIN));
  lcd.print(" ");
  lcd.print(lastMode);    // ex: AUTO

  delay(5000); // așteptăm 5 secunde
}

void blinkLed() {
  for (int i = 0; i < 3; i++) {
    digitalWrite(LED_PIN, HIGH);
    delay(200);
    digitalWrite(LED_PIN, LOW);
    delay(200);
  }
}
