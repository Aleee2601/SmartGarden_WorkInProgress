#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>

// 🔧 Config Wi-Fi
const char* ssid = "Alexandra";
const char* password = "26012005";

// 🔧 Server HTTP
ESP8266WebServer server(80);

// 📦 Ultimul mesaj JSON de la Arduino
String lastSensorData = "{}";

void setup() {
  Serial.begin(9600); // UART cu Arduino

  WiFi.begin(ssid, password);
  Serial.println("Conectare Wi-Fi...");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("\n✅ Conectat la Wi-Fi!");
  Serial.print("IP ESP: ");
  Serial.println(WiFi.localIP());

  // 📡 GET /data - trimite datele senzorilor
  server.on("/data", HTTP_GET, []() {
    server.send(200, "application/json", lastSensorData);
  });

  // 🧠 POST /water - trimite comanda pompei la Arduino
  server.on("/water", HTTP_POST, []() {
    if (!server.hasArg("plain")) {
      server.send(400, "text/plain", "Fara continut");
      return;
    }
    String cmd = server.arg("plain");
    if (cmd == "WATER ON" || cmd == "WATER OFF") {
      Serial.println(cmd);
      server.send(200, "text/plain", "Comanda trimisa: " + cmd);
    } else {
      server.send(400, "text/plain", "Comanda invalida");
    }
  });

  // 🔁 POST /status - trimite status la Arduino (pt LCD)
  server.on("/status", HTTP_POST, []() {
    if (!server.hasArg("plain")) {
      server.send(400, "text/plain", "Fara continut");
      return;
    }
    String status = server.arg("plain");
    Serial.println("STATUS:" + status);
    server.send(200, "text/plain", "Status trimis");
  });

  // 🔁 POST /mode - trimite mod la Arduino (pt LCD)
  server.on("/mode", HTTP_POST, []() {
    if (!server.hasArg("plain")) {
      server.send(400, "text/plain", "Fara continut");
      return;
    }
    String mode = server.arg("plain");
    Serial.println("MODE:" + mode);
    server.send(200, "text/plain", "Mod trimis");
  });

  // 🔁 POST /time - trimite ora la Arduino (pt LCD)
  server.on("/time", HTTP_POST, []() {
    if (!server.hasArg("plain")) {
      server.send(400, "text/plain", "Fara continut");
      return;
    }
    String time = server.arg("plain");
    Serial.println("TIME:" + time);
    server.send(200, "text/plain", "Ora trimisa");
  });

  server.begin();
  Serial.println("🌐 Server HTTP pornit");
}

void loop() {
  server.handleClient();

  // 🔁 Citim datele primite de la Arduino
  if (Serial.available()) {
    String data = Serial.readStringUntil('\n');
    data.trim();
    if (data.startsWith("{")) {
      lastSensorData = data;
      Serial.println("[ESP] Date salvate: " + data); // pentru debug
    }
  }
}
