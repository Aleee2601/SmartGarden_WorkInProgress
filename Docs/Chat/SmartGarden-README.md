# SmartGarden — Full-Stack IoT (C#/.NET)
Opinionated starter plan and commands for a production-lean MVP: Android/iOS app (.NET MAUI) + ASP.NET Core API + MQTT (Mosquitto) + SQL Server + ESP32 + CI/CD.

## 1) Tech Stack (final)
**Frontend (mobile):** .NET MAUI • MVVM (CommunityToolkit.Mvvm) • Refit (REST client) • SecureStorage + SQLite • Microcharts/SkiaSharp • SignalR (push) • *(optional)* MQTT read-only  
**Backend:** ASP.NET Core Web API (.NET 8) • Entity Framework Core + SQL Server • JWT + refresh • FluentValidation • Serilog • SignalR • MQTTnet (consumer & publisher) • Hangfire (jobs)  
**Broker MQTT:** Mosquitto (TLS, auth, ACL) or EMQX (enterprise)  
**Databases:** SQL Server (prod) • SQLite (local/integration tests)  
**ESP32 firmware:** PlatformIO/Arduino • AsyncMqttClient • ArduinoJson • WiFiClientSecure (TLS)  
**Observability:** OpenTelemetry (traces/metrics) • Serilog → Seq • HealthChecks UI  
**Testing:** xUnit • FluentAssertions • Testcontainers (.NET) • *(optional)* WireMock.Net; MAUI Test Runner  
**DevOps:** Docker + Docker Compose • GitHub Actions (CI: build+test → coverage; CD: build+push images → deploy)

---

## 2) Repository Layout
```
smartgarden/
├─ infrastructure/
│  └─ docker/                 # compose, mosquitto config, seq, etc.
├─ src/
│  ├─ SmartGarden.Api/        # ASP.NET Core API (Swagger, Auth, SignalR)
│  ├─ SmartGarden.Application/# DTOs, UseCases, Validators
│  ├─ SmartGarden.Domain/     # Entities, domain logic
│  ├─ SmartGarden.Infrastructure/ # EF Core, Repos, Hangfire, MQTT abstractions
│  ├─ SmartGarden.MqttWorker/ # Hosted MQTT consumer/publisher
│  └─ SmartGarden.Maui/       # .NET MAUI mobile app
├─ firmware/
│  └─ esp32-smartgarden/      # PlatformIO/Arduino code
└─ tests/
   ├─ SmartGarden.UnitTests/
   └─ SmartGarden.IntegrationTests/
```

---

## 3) Quickstart (Dev env)
### Prereqs
- .NET SDK 8.x  
- Docker Desktop (or CLI)  
- Android SDK (for MAUI), Xcode (for iOS)  
- GitHub account (for Actions)

### One-time: create solution & projects
```bash
dotnet new sln -n SmartGarden

dotnet new webapi -n SmartGarden.Api -o src/SmartGarden.Api
dotnet new classlib -n SmartGarden.Application -o src/SmartGarden.Application
dotnet new classlib -n SmartGarden.Domain -o src/SmartGarden.Domain
dotnet new classlib -n SmartGarden.Infrastructure -o src/SmartGarden.Infrastructure
dotnet new worker -n SmartGarden.MqttWorker -o src/SmartGarden.MqttWorker
dotnet new maui -n SmartGarden.Maui -o src/SmartGarden.Maui

dotnet sln add src/**/*
dotnet add src/SmartGarden.Api reference src/SmartGarden.Application src/SmartGarden.Infrastructure src/SmartGarden.Domain
dotnet add src/SmartGarden.Application reference src/SmartGarden.Domain
dotnet add src/SmartGarden.Infrastructure reference src/SmartGarden.Domain
dotnet add src/SmartGarden.MqttWorker reference src/SmartGarden.Infrastructure src/SmartGarden.Application src/SmartGarden.Domain
```

### NuGet (core packages)
```bash
# API / Infra
dotnet add src/SmartGarden.Api package Serilog.AspNetCore
dotnet add src/SmartGarden.Api package Swashbuckle.AspNetCore
dotnet add src/SmartGarden.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/SmartGarden.Api package Microsoft.AspNetCore.RateLimiting
dotnet add src/SmartGarden.Api package AspNetCore.HealthChecks.UI.Client
dotnet add src/SmartGarden.Api package Microsoft.AspNetCore.SignalR

dotnet add src/SmartGarden.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/SmartGarden.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/SmartGarden.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/SmartGarden.Infrastructure package FluentValidation
dotnet add src/SmartGarden.Infrastructure package MQTTnet
dotnet add src/SmartGarden.Infrastructure package Hangfire
dotnet add src/SmartGarden.Infrastructure package Hangfire.SqlServer
dotnet add src/SmartGarden.Infrastructure package OpenTelemetry.Extensions.Hosting
dotnet add src/SmartGarden.Infrastructure package Serilog.Sinks.Seq

# MQTT Worker
dotnet add src/SmartGarden.MqttWorker package MQTTnet
dotnet add src/SmartGarden.MqttWorker package Serilog.Sinks.Seq
dotnet add src/SmartGarden.MqttWorker package OpenTelemetry.Extensions.Hosting

# MAUI
dotnet add src/SmartGarden.Maui package CommunityToolkit.Mvvm
dotnet add src/SmartGarden.Maui package Refit
dotnet add src/SmartGarden.Maui package Microcharts
dotnet add src/SmartGarden.Maui package SkiaSharp.Views.Maui.Controls
dotnet add src/SmartGarden.Maui package Microsoft.AspNetCore.SignalR.Client
```

### Docker Compose (dev)
Create `infrastructure/docker/docker-compose.dev.yml` with SQL Server, Mosquitto, Seq, API, MQTT worker. Start everything:
```bash
cd infrastructure/docker
docker compose -f docker-compose.dev.yml up -d --build
```

### Database (dev)
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -p src/SmartGarden.Infrastructure -s src/SmartGarden.Api
dotnet ef database update -p src/SmartGarden.Infrastructure -s src/SmartGarden.Api
```

### Run API locally
```bash
dotnet run --project src/SmartGarden.Api
# Swagger: http://localhost:5000/swagger
```

### Run MAUI (Android emulator)
```bash
dotnet build src/SmartGarden.Maui
dotnet run --project src/SmartGarden.Maui -f net8.0-android
```

---

## 4) MQTT Contract (topics & payloads)
- smartgarden/{{tenant}}/{{deviceId}}/telemetry (QoS1)  
- smartgarden/{{tenant}}/{{deviceId}}/state (retained+QoS1)  
- smartgarden/{{tenant}}/{{deviceId}}/cmd/irrigate (QoS1)  
- smartgarden/{{tenant}}/{{deviceId}}/evt/alert (QoS1)

telemetry:
```json
{ "ts": "2025-10-14T12:34:56Z",
  "sensors": {"soil":0.43,"temp":23.6,"hum":45.2,"lux":512,"tank":72},
  "rssi": -58, "vbat": 4.02 }
```
state (retained):
```json
{ "deviceId":"esp32-1234","online":true,"lastSeen":"2025-10-14T12:35:01Z",
  "mode":"auto","pump":{"isOn":false,"lastRunSec":6} }
```
cmd/irrigate:
```json
{ "requestId":"d9c2f1","durationSec":5 }
```

---

## 5) Security (must-do)
- JWT + refresh tokens (API) • password hashing (Argon2/PBKDF2)  
- Mosquitto: users/passwords in passwd + aclfile per device • enable TLS in prod (8883)  
- CORS strict, rate limiting, input validation (FluentValidation)

---

## 6) Testing
- Unit: xUnit + FluentAssertions (Domain/Application)  
- Integration: Testcontainers (SQL Server + Mosquitto) → ingest fake telemetry and assert DB writes  
- MAUI: unit tests on ViewModels; optional UI runner

---

## 7) Observability
- Serilog → Seq (docker)
- OpenTelemetry (HTTP, DB; custom spans for MQTT)
- HealthChecks UI (SQL/MQTT/SignalR)

---

## 8) CI (GitHub Actions)
Minimal CI:
```yaml
name: CI
on: [push, pull_request]
jobs:
  build_test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: 
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release --collect "XPlat Code Coverage"
```
CD: build & push Docker images to GHCR/DockerHub; deploy to Azure Container Apps/AKS.

---

## 9) ESP32 (firmware tips)
- Use PlatformIO + AsyncMqttClient + ArduinoJson  
- Set LWT: retained state.online=false  
- QoS1 for telemetry/cmd; deduplicate by requestId  
- Hardware: MOSFET logic-level + diode flyback; capacitive soil sensor; level-shift for ultrasonic if 5V

Happy building! 🚀
