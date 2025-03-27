#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>

#define DHTPIN 2            // Senzorul DHT este conectat la GPIO2
#define DHTTYPE DHT11       // Sau DHT22, în funcție de senzor
DHT dht(DHTPIN, DHTTYPE);

const char* ssid = "Alexandra";
const char* password = "26012005";
const char* serverName = "http://api.thingspeak.com/update?api_key=A5Y82K4CRO14YGFS";

void setup() {
  Serial.begin(115200);
  dht.begin();

  WiFi.begin(ssid, password);
  Serial.print("Se conectează la WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println();
  Serial.println("WiFi conectat!");
}

void loop() {
  float temperature = dht.readTemperature();
  float humidity = dht.readHumidity();

  if (isnan(temperature) || isnan(humidity)) {
    Serial.println("Eroare la citirea senzorului DHT!");
    delay(2000);
    return;
  }

  if (WiFi.status() == WL_CONNECTED) {
    WiFiClient client;
    HTTPClient http;
    // Construiește URL-ul cu parametrii
    String url = String(serverName) + "&field1=" + String(temperature) + "&field2=" + String(humidity);
    
    // Folosește noul API: transmite clientul și URL-ul
    http.begin(client, url);
    int httpCode = http.GET();

    if (httpCode > 0) {
      Serial.print("Cod HTTP: ");
      Serial.println(httpCode);
      String payload = http.getString();
      Serial.println("Răspuns:");
      Serial.println(payload);
    } else {
      Serial.print("Eroare la cererea HTTP: ");
      Serial.println(http.errorToString(httpCode));
    }
    http.end();
  }
  
  delay(10000);
}
