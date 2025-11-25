# SmartGarden Deployment Guide 🚀

## Overview

This guide covers deploying the SmartGarden IoT system in various environments: development, staging, and production. It includes Docker containerization, cloud deployment (Azure, AWS), and CI/CD setup.

## Table of Contents

1. [Deployment Options](#deployment-options)
2. [Docker Deployment](#docker-deployment)
3. [Azure Deployment](#azure-deployment)
4. [AWS Deployment](#aws-deployment)
5. [Manual Deployment](#manual-deployment)
6. [CI/CD Pipeline](#cicd-pipeline)
7. [Environment Configuration](#environment-configuration)
8. [Monitoring & Logging](#monitoring--logging)
9. [Security Best Practices](#security-best-practices)
10. [Troubleshooting](#troubleshooting)

## Deployment Options

### Development
- LocalDB or SQL Server Express
- IIS Express or Kestrel
- npm run dev for frontend
- Manual ESP32 flashing

### Staging
- Docker Compose
- Azure App Service (Free/Basic tier)
- SQL Server on Docker
- Automated builds

### Production
- Kubernetes or Docker Swarm
- Azure App Service (Standard/Premium)
- Azure SQL Database
- CDN for frontend assets
- OTA updates for ESP32

## Docker Deployment

### Prerequisites

```bash
# Install Docker Desktop
# Windows: https://www.docker.com/products/docker-desktop
# Linux: apt-get install docker.io docker-compose

# Verify installation
docker --version
docker-compose --version
```

### Backend API Dockerfile

Create `MobileApp/SmartGarden/SmartGarden.Project/Dockerfile`:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["SmartGarden.API/SmartGarden.API.csproj", "SmartGarden.API/"]
COPY ["SmartGarden.Core/SmartGarden.Core.csproj", "SmartGarden.Core/"]
COPY ["SmartGarden.Data/SmartGarden.Data.csproj", "SmartGarden.Data/"]

# Restore dependencies
RUN dotnet restore "SmartGarden.API/SmartGarden.API.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/SmartGarden.API"
RUN dotnet build "SmartGarden.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SmartGarden.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SmartGarden.API.dll"]
```

### Frontend Dockerfile

Create `ReactNativeApp/SmartGardenApp/Dockerfile`:

```dockerfile
# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production

# Copy source code
COPY . .

# Build application
RUN npm run build

# Runtime stage
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy build files
COPY --from=build /app/dist .

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

### Nginx Configuration

Create `ReactNativeApp/SmartGardenApp/nginx.conf`:

```nginx
server {
    listen 80;
    server_name _;
    root /usr/share/nginx/html;
    index index.html;

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml text/javascript;

    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # SPA routing
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy
    location /api {
        proxy_pass http://backend:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # SignalR WebSocket
    location /hubs {
        proxy_pass http://backend:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### Docker Compose

Create `docker-compose.yml` in root:

```yaml
version: '3.8'

services:
  # SQL Server Database
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: smartgarden-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd123
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - smartgarden-network
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd123" -Q "SELECT 1"
      interval: 30s
      timeout: 10s
      retries: 5

  # Backend API
  backend:
    build:
      context: ./MobileApp/SmartGarden/SmartGarden.Project
      dockerfile: Dockerfile
    container_name: smartgarden-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Server=database;Database=SmartGardenDb;User Id=sa;Password=YourStrong@Passw0rd123;TrustServerCertificate=true
      - JwtSettings__Secret=your-super-secret-key-minimum-32-characters-long
      - JwtSettings__Issuer=SmartGardenAPI
      - JwtSettings__Audience=SmartGardenApp
      - JwtSettings__ExpiryMinutes=1440
      - PerenualApiKey=${PERENUAL_API_KEY}
    ports:
      - "5000:8080"
    depends_on:
      database:
        condition: service_healthy
    networks:
      - smartgarden-network
    restart: unless-stopped
    healthcheck:
      test: curl -f http://localhost:8080/health || exit 1
      interval: 30s
      timeout: 10s
      retries: 3

  # Frontend Web App
  frontend:
    build:
      context: ./ReactNativeApp/SmartGardenApp
      dockerfile: Dockerfile
    container_name: smartgarden-web
    environment:
      - VITE_API_URL=http://localhost:5000/api
      - VITE_SIGNALR_HUB_URL=http://localhost:5000/hubs/plant
    ports:
      - "3000:80"
    depends_on:
      - backend
    networks:
      - smartgarden-network
    restart: unless-stopped

volumes:
  sqldata:
    driver: local

networks:
  smartgarden-network:
    driver: bridge
```

### Build and Run

```bash
# Build images
docker-compose build

# Start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

### Environment Variables

Create `.env` file in root:

```env
# Database
SQL_PASSWORD=YourStrong@Passw0rd123

# API Keys
PERENUAL_API_KEY=your-perenual-api-key

# JWT
JWT_SECRET=your-super-secret-key-minimum-32-characters-long
JWT_ISSUER=SmartGardenAPI
JWT_AUDIENCE=SmartGardenApp

# Email (optional)
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

Load in docker-compose:

```yaml
services:
  backend:
    env_file:
      - .env
    environment:
      - ConnectionStrings__DefaultConnection=Server=database;Database=SmartGardenDb;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=true
      - PerenualApiKey=${PERENUAL_API_KEY}
```

## Azure Deployment

### Prerequisites

```bash
# Install Azure CLI
# Windows: https://aka.ms/installazurecliwindows
# Linux: curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login
az login

# Set subscription
az account set --subscription "Your Subscription Name"
```

### Create Azure Resources

```bash
# Variables
RESOURCE_GROUP="smartgarden-rg"
LOCATION="eastus"
APP_SERVICE_PLAN="smartgarden-plan"
WEB_APP_NAME="smartgarden-api-12345"
SQL_SERVER="smartgarden-sqlserver"
SQL_DATABASE="SmartGardenDb"
SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="YourStrong@Passw0rd123"

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create App Service Plan (B1 - Basic)
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku B1 \
  --is-linux

# Create Web App for Backend
az webapp create \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE:10.0"

# Create SQL Server
az sql server create \
  --name $SQL_SERVER \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN_USER \
  --admin-password $SQL_ADMIN_PASSWORD

# Configure firewall (allow Azure services)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Create SQL Database (Basic tier)
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $SQL_DATABASE \
  --service-objective Basic

# Get connection string
az sql db show-connection-string \
  --client ado.net \
  --server $SQL_SERVER \
  --name $SQL_DATABASE
```

### Configure App Settings

```bash
# Set connection string
az webapp config connection-string set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=$SQL_DATABASE;User ID=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;Encrypt=true;Connection Timeout=30;"

# Set app settings
az webapp config appsettings set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    JwtSettings__Secret="your-super-secret-key" \
    JwtSettings__Issuer="SmartGardenAPI" \
    JwtSettings__Audience="SmartGardenApp" \
    PerenualApiKey="your-api-key"

# Enable HTTPS only
az webapp update \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --https-only true

# Enable WebSocket (for SignalR)
az webapp config set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --web-sockets-enabled true
```

### Deploy Backend

```bash
# From project directory
cd MobileApp/SmartGarden/SmartGarden.Project

# Publish application
dotnet publish SmartGarden.API/SmartGarden.API.csproj \
  -c Release \
  -o ./publish

# Create deployment zip
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to Azure
az webapp deploy \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --src-path deploy.zip \
  --type zip
```

### Deploy Frontend (Static Web App)

```bash
# Create Static Web App
az staticwebapp create \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

# Build frontend
cd ReactNativeApp/SmartGardenApp
npm run build

# Deploy (via GitHub Actions or Azure CLI)
# Option 1: Azure CLI
az staticwebapp deploy \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --app-location ./dist
```

### Run Migrations

```bash
# From SmartGarden.Data directory
dotnet ef database update \
  --startup-project ../SmartGarden.API \
  --connection "Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=$SQL_DATABASE;User ID=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;Encrypt=true;"
```

## AWS Deployment

### Prerequisites

```bash
# Install AWS CLI
# https://aws.amazon.com/cli/

# Configure AWS credentials
aws configure
```

### Deploy with Elastic Beanstalk

```bash
# Install EB CLI
pip install awsebcli

# Initialize Elastic Beanstalk
cd MobileApp/SmartGarden/SmartGarden.Project
eb init smartgarden-api --platform ".NET Core 10" --region us-east-1

# Create environment
eb create smartgarden-prod --instance-type t3.small

# Deploy
dotnet publish -c Release
eb deploy

# Set environment variables
eb setenv \
  ASPNETCORE_ENVIRONMENT=Production \
  JwtSettings__Secret="your-secret" \
  PerenualApiKey="your-key"
```

### RDS Database

```bash
# Create RDS SQL Server instance
aws rds create-db-instance \
  --db-instance-identifier smartgarden-db \
  --db-instance-class db.t3.small \
  --engine sqlserver-ex \
  --master-username admin \
  --master-user-password YourPassword123 \
  --allocated-storage 20
```

## Manual Deployment

### Windows Server with IIS

#### 1. Install Prerequisites

- Install IIS with ASP.NET Core module
- Install .NET 10 Hosting Bundle
- Install SQL Server

#### 2. Publish Application

```bash
cd MobileApp/SmartGarden/SmartGarden.Project
dotnet publish SmartGarden.API/SmartGarden.API.csproj -c Release -o C:\inetpub\smartgarden
```

#### 3. Create IIS Site

```powershell
# Import IIS module
Import-Module WebAdministration

# Create application pool
New-WebAppPool -Name "SmartGardenPool"
Set-ItemProperty IIS:\AppPools\SmartGardenPool -Name "managedRuntimeVersion" -Value ""

# Create website
New-Website -Name "SmartGarden" `
  -PhysicalPath "C:\inetpub\smartgarden" `
  -ApplicationPool "SmartGardenPool" `
  -Port 80

# Enable WebSocket
Set-WebConfigurationProperty `
  -Filter /system.webServer/webSocket `
  -Name enabled `
  -Value true `
  -PSPath "IIS:\Sites\SmartGarden"
```

#### 4. Configure SSL

```powershell
# Bind SSL certificate
New-WebBinding -Name "SmartGarden" -IP "*" -Port 443 -Protocol https
Get-ChildItem cert:\LocalMachine\My | Where-Object {$_.Subject -like "*smartgarden*"} |
  Select-Object -First 1 |
  New-Item IIS:\SslBindings\0.0.0.0!443
```

### Linux Server (Ubuntu)

#### 1. Install .NET 10

```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0
```

#### 2. Install Nginx

```bash
sudo apt update
sudo apt install nginx
```

#### 3. Configure Nginx

```bash
sudo nano /etc/nginx/sites-available/smartgarden
```

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    location /hubs {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/smartgarden /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### 4. Create Systemd Service

```bash
sudo nano /etc/systemd/system/smartgarden.service
```

```ini
[Unit]
Description=SmartGarden API
After=network.target

[Service]
WorkingDirectory=/var/www/smartgarden
ExecStart=/usr/bin/dotnet /var/www/smartgarden/SmartGarden.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=smartgarden-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl enable smartgarden
sudo systemctl start smartgarden
sudo systemctl status smartgarden
```

## CI/CD Pipeline

### GitHub Actions

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy SmartGarden

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'

    - name: Restore dependencies
      run: dotnet restore MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/SmartGarden.API.csproj

    - name: Build
      run: dotnet build MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/SmartGarden.API.csproj --no-restore -c Release

    - name: Test
      run: dotnet test MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.Tests/SmartGarden.Tests.csproj --no-build -c Release

    - name: Publish
      run: dotnet publish MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/SmartGarden.API.csproj -c Release -o ./publish

    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: api-artifact
        path: ./publish

  deploy-to-azure:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
    - name: Download artifact
      uses: actions/download-artifact@v3
      with:
        name: api-artifact

    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'smartgarden-api'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: .

  build-frontend:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'

    - name: Install dependencies
      run: |
        cd ReactNativeApp/SmartGardenApp
        npm ci

    - name: Build
      run: |
        cd ReactNativeApp/SmartGardenApp
        npm run build

    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: frontend-artifact
        path: ReactNativeApp/SmartGardenApp/dist

  deploy-frontend:
    needs: build-frontend
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'

    steps:
    - name: Download artifact
      uses: actions/download-artifact@v3
      with:
        name: frontend-artifact

    - name: Deploy to Azure Static Web Apps
      uses: Azure/static-web-apps-deploy@v1
      with:
        azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
        action: "upload"
        app_location: "."
        skip_app_build: true
```

## Environment Configuration

### Development

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartGardenDb;Trusted_Connection=true"
  },
  "JwtSettings": {
    "Secret": "development-secret-key-32-characters",
    "ExpiryMinutes": 1440
  }
}
```

### Production

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=SmartGardenDb;User Id=sa;Password=xxx"
  },
  "JwtSettings": {
    "Secret": "${JWT_SECRET}",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

## Monitoring & Logging

### Application Insights (Azure)

```bash
# Add package
dotnet add package Microsoft.ApplicationInsights.AspNetCore

# Configure in Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SmartGardenContext>()
    .AddUrlGroup(new Uri("https://api.perenual.com/api/species-list"), "Perenual API");

app.MapHealthChecks("/health");
```

### Structured Logging

```csharp
// Use Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/smartgarden-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
    .CreateLogger();
```

## Security Best Practices

1. **Use HTTPS everywhere**
2. **Store secrets in Azure Key Vault** or AWS Secrets Manager
3. **Enable CORS** only for trusted origins
4. **Use managed identities** for Azure resources
5. **Implement rate limiting**
6. **Regular security updates**
7. **Enable WAF** (Web Application Firewall)
8. **Database encryption** at rest and in transit

## Troubleshooting

### Docker Issues

```bash
# View container logs
docker-compose logs backend

# Restart specific service
docker-compose restart backend

# Check container health
docker ps
docker inspect smartgarden-api
```

### Azure Issues

```bash
# View app logs
az webapp log tail --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP

# Check deployment status
az webapp deployment list --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP

# Restart app
az webapp restart --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP
```

## Related Documentation

- [Main Project Documentation](./PROJECT_DOCUMENTATION.md)
- [Backend API README](./MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/README.md)
- [Database README](./DATABASE_README.md)

---

**Version:** 1.0
**Last Updated:** November 2025
**Supported Platforms:** Docker, Azure, AWS, IIS, Linux
