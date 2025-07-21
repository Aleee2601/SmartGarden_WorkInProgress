#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>

const char* ssid = "Alexandra";
const char* password = "26012005";

ESP8266WebServer server(80);

String lastSensorData = "{}";

void setup() {
  Serial.begin(9600);  // Communicate with Arduino
  WiFi.begin(ssid, password);

  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected!");
  Serial.print("ESP IP: ");
  Serial.println(WiFi.localIP());

  // Endpoint: GET /data
  server.on("/data", HTTP_GET, []() {
    server.send(200, "application/json", lastSensorData);
  });

  // Endpoint: POST /water
  server.on("/water", HTTP_POST, []() {
    if (!server.hasArg("plain")) {
      server.send(400, "text/plain", "Missing body");
      return;
    }

    String command = server.arg("plain");
    command.trim();

    if (command == "WATER ON" || command == "WATER OFF") {
      Serial.println(command);  // Send to Arduino
      server.send(200, "text/plain", "Command sent: " + command);
    } else {
      server.send(400, "text/plain", "Invalid command");
    }
  });

  server.begin();
  Serial.println("HTTP server started");
}

void loop() {
  server.handleClient();

  // Read sensor data from Arduino
  if (Serial.available()) {
    lastSensorData = Serial.readStringUntil('\n');
    lastSensorData.trim();
    Serial.println("[ESP] Received from Arduino: " + lastSensorData);
  }
}
