#include <SoftwareSerial.h>
#include <DHT.h>

// Definirea pinului și tipului de senzor DHT (temperatură și umiditate aer)
#define DHTPIN A0           // Senzorul DHT este conectat la A0
#define DHTTYPE DHT11       // Poate fi DHT11 sau DHT22, în funcție de senzorul tău
DHT dht(DHTPIN, DHTTYPE);

// Senzor HW080 (umiditate în sol) – presupunem că are ieșire analogică
#define SOIL_MOISTURE_PIN A1

// Prag pentru umiditatea solului (valoarea se ajustează în funcție de calibrare)
// De obicei, o valoare mai mare indică sol mai uscat.
const int soilThreshold = 700;

// Definirea pinului pentru controlul pompei de apă prin releu/MOS module
#define PUMP_PIN 8

// Configurare SoftwareSerial pentru ESP-01
// Pin 10 = RX, 11 = TX pentru comunicarea cu ESP-01
SoftwareSerial espSerial(10, 11);

// Detalii rețea WiFi și ThingSpeak
const char* ssid = "Alexandra";
const char* password = "26012005";
const char* host = "api.thingspeak.com";  // Host ThingSpeak
const int httpPort = 80;

// API Key pentru ThingSpeak (folosește Write API Key de la canalul tău)
const char* apiKey = "A5Y82K4CRO14YGFS";

void setup() {
  Serial.begin(9600);       // Monitor serial pentru debug
  espSerial.begin(9600);    // Comunicare cu ESP-01
  dht.begin();
  
  // Configurarea pinului de control al pompei
  pinMode(PUMP_PIN, OUTPUT);
  digitalWrite(PUMP_PIN, LOW);  // Pornirea este LOW (de obicei, LOW = pompa oprită; verifică specificațiile modulului tău)

  Serial.println("Restart modul ESP...");
  sendATCommand("AT+RST\r\n", 2000);
  delay(2000);
  
  Serial.println("Setez modul WiFi in STA (station)...");
  sendATCommand("AT+CWMODE=1\r\n", 1000);
  delay(1000);
  
  // Conectare la rețeaua WiFi
  String cmd = "AT+CWJAP=\"" + String(ssid) + "\",\"" + String(password) + "\"\r\n";
  Serial.print("Conectez la WiFi: ");
  Serial.println(cmd);
  sendATCommand(cmd.c_str(), 5000);
  delay(2000);
}

void loop() {
  // Citește temperatura și umiditatea din aer de la senzorul DHT
  float humidity = dht.readHumidity();
  float temperature = dht.readTemperature();
  
  if (isnan(humidity) || isnan(temperature)) {
    Serial.println("Eroare la citirea senzorului DHT!");
    delay(2000);
    return;
  }
  
  // Citește umiditatea solului de la senzorul HW080
  int soilMoisture = analogRead(SOIL_MOISTURE_PIN);
  
  // Controlează pompa: dacă solul este prea uscat (valoare peste prag), activează pompa
  if (soilMoisture > soilThreshold) {
    digitalWrite(PUMP_PIN, HIGH); // Activează pompa
    Serial.println("Sol uscat - pompa pornită");
  } else {
    digitalWrite(PUMP_PIN, LOW);  // Dezactivează pompa
    Serial.println("Sol umed - pompa oprită");
  }
  
  // Construiește cererea HTTP GET pentru ThingSpeak cu datele de la senzori:
  // Field1 = temperatură, Field2 = umiditate aer, Field3 = umiditate sol
  String httpRequest = "GET /update?api_key=" + String(apiKey) +
                       "&field1=" + String(temperature) +
                       "&field2=" + String(humidity) +
                       "&field3=" + String(soilMoisture) +
                       " HTTP/1.1\r\n" +
                       "Host: " + String(host) + "\r\n" +
                       "Connection: close\r\n\r\n";
  
  Serial.println("Se conectează la server...");
  String cmd = "AT+CIPSTART=\"TCP\",\"" + String(host) + "\"," + String(httpPort) + "\r\n";
  sendATCommand(cmd.c_str(), 2000);
  
  // Trimite lungimea datelor pe care le vei transmite
  int length = httpRequest.length();
  cmd = "AT+CIPSEND=" + String(length) + "\r\n";
  sendATCommand(cmd.c_str(), 2000);
  
  // Trimite cererea HTTP
  Serial.println("Trimit cererea HTTP:");
  Serial.println(httpRequest);
  sendATCommand(httpRequest.c_str(), 2000);
  
  // Închide conexiunea
  sendATCommand("AT+CIPCLOSE\r\n", 1000);
  
  // Așteaptă 10 secunde înainte de următoarea actualizare
  delay(10000);
}

// Funcție pentru trimiterea comenzilor AT și afișarea răspunsului de la ESP
void sendATCommand(const char* command, unsigned long timeout) {
  Serial.print("Comanda AT: ");
  Serial.println(command);
  espSerial.print(command);
  
  unsigned long t = millis();
  while (millis() - t < timeout) {
    while (espSerial.available()) {
      char c = espSerial.read();
      Serial.write(c);
    }
  }
  Serial.println();
}
