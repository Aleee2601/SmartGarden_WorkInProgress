# SmartGarden Cloud Deployment Guide - Quick Start ☁️

## Overview

This guide will walk you through deploying your SmartGarden app to **Microsoft Azure** (best for .NET apps). You'll deploy:

1. **Backend API** → Azure App Service
2. **Database** → Azure SQL Database
3. **Frontend** → Azure Static Web Apps (or App Service)
4. **ESP32 devices** → Connect to your cloud API

**Estimated Time:** 30-45 minutes
**Estimated Monthly Cost:** $5-15 (Free tier available)

---

## Option 1: Azure Deployment (Recommended) 💙

### Prerequisites

1. **Azure Account**
   - Sign up at https://azure.microsoft.com/free/
   - Get $200 free credit for 30 days
   - Free tier includes many services

2. **Install Azure CLI**

   **Windows:**
   ```bash
   # Download and run installer
   https://aka.ms/installazurecliwindows
   ```

   **macOS:**
   ```bash
   brew update && brew install azure-cli
   ```

   **Linux:**
   ```bash
   curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
   ```

3. **Verify Installation**
   ```bash
   az --version
   ```

### Step 1: Login to Azure

```bash
# Login to your Azure account
az login

# List your subscriptions
az account list --output table

# Set active subscription (if you have multiple)
az account set --subscription "Your Subscription Name"
```

### Step 2: Create Azure Resources

```bash
# Set variables (CHANGE THESE!)
RESOURCE_GROUP="smartgarden-rg"
LOCATION="eastus"  # or "westeurope", "southeastasia", etc.
APP_NAME="smartgarden-api-$(date +%s)"  # Unique name
SQL_SERVER="smartgarden-sql-$(date +%s)"
SQL_DATABASE="SmartGardenDb"
SQL_ADMIN="sqladmin"
SQL_PASSWORD="YourStrong@Password123"  # CHANGE THIS!

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

echo "✅ Resource group created!"
```

### Step 3: Create SQL Database

```bash
# Create SQL Server
az sql server create \
  --name $SQL_SERVER \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN \
  --admin-password $SQL_PASSWORD

# Allow Azure services to access SQL Server
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow your IP address (for management)
MY_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowMyIP \
  --start-ip-address $MY_IP \
  --end-ip-address $MY_IP

# Create SQL Database (Basic tier - cheapest)
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $SQL_DATABASE \
  --service-objective Basic \
  --backup-storage-redundancy Local

echo "✅ SQL Database created!"
echo "Connection string: Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=$SQL_DATABASE;User ID=$SQL_ADMIN;Password=$SQL_PASSWORD;Encrypt=true;"
```

### Step 4: Deploy Backend API

```bash
# Create App Service Plan (B1 - Basic tier)
az appservice plan create \
  --name smartgarden-plan \
  --resource-group $RESOURCE_GROUP \
  --sku B1 \
  --is-linux

# Create Web App
az webapp create \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --plan smartgarden-plan \
  --runtime "DOTNETCORE:8.0"

# Enable WebSocket (required for SignalR)
az webapp config set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --web-sockets-enabled true

# Enable HTTPS only
az webapp update \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --https-only true

echo "✅ Web App created!"
echo "URL: https://$APP_NAME.azurewebsites.net"
```

### Step 5: Configure App Settings

```bash
# Set connection string
az webapp config connection-string set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=$SQL_DATABASE;User ID=$SQL_ADMIN;Password=$SQL_PASSWORD;Encrypt=true;Connection Timeout=30;"

# Set application settings
az webapp config appsettings set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    JwtSettings__Secret="your-super-secret-jwt-key-must-be-at-least-32-characters-long-12345" \
    JwtSettings__Issuer="SmartGardenAPI" \
    JwtSettings__Audience="SmartGardenApp" \
    JwtSettings__ExpiryMinutes="1440" \
    PerenualApiKey="YOUR_PERENUAL_API_KEY_HERE"

echo "✅ App settings configured!"
```

### Step 6: Build and Deploy Backend

```bash
# Navigate to your project directory
cd ~/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project

# Publish the application
dotnet publish SmartGarden.API/SmartGarden.API.csproj \
  -c Release \
  -o ./publish

# Create deployment zip
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to Azure
az webapp deploy \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --src-path deploy.zip \
  --type zip

echo "✅ Backend deployed!"
echo "API URL: https://$APP_NAME.azurewebsites.net"
```

### Step 7: Run Database Migrations

```bash
# Navigate to Data project
cd ~/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.Data

# Update database with migrations
CONNECTION_STRING="Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=$SQL_DATABASE;User ID=$SQL_ADMIN;Password=$SQL_PASSWORD;Encrypt=true;Connection Timeout=30;"

dotnet ef database update \
  --startup-project ../SmartGarden.API \
  --connection "$CONNECTION_STRING"

echo "✅ Database migrations applied!"
```

### Step 8: Deploy Frontend (Static Web App)

```bash
# Create Static Web App
az staticwebapp create \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

# Get deployment token
DEPLOYMENT_TOKEN=$(az staticwebapp secrets list \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --query "properties.apiKey" -o tsv)

# Build frontend
cd ~/SmartGarden_WorkInProgress/ReactNativeApp/SmartGardenApp

# Update API URL in environment
cat > .env.production <<EOF
VITE_API_URL=https://$APP_NAME.azurewebsites.net/api
VITE_SIGNALR_HUB_URL=https://$APP_NAME.azurewebsites.net/hubs/plant
EOF

# Build
npm install
npm run build

# Deploy using Azure CLI
az staticwebapp deploy \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --app-location ./dist \
  --deployment-token $DEPLOYMENT_TOKEN

echo "✅ Frontend deployed!"
```

### Step 9: Configure ESP32 Devices

Update your ESP32 firmware configuration:

```cpp
// In FirmWare/SecureESP32/config.h

// WiFi Configuration
const char* WIFI_SSID = "YourWiFiName";
const char* WIFI_PASSWORD = "YourWiFiPassword";

// API Configuration
const char* API_BASE_URL = "https://YOUR-APP-NAME.azurewebsites.net/api";
const char* API_REGISTER_ENDPOINT = "/auth/device/register";
const char* API_LOGIN_ENDPOINT = "/auth/device/login";
const char* API_TELEMETRY_ENDPOINT = "/telemetry";
```

Flash the updated firmware to your ESP32.

### Step 10: Test Your Deployment

```bash
# Test API health
curl https://$APP_NAME.azurewebsites.net/health

# Test frontend
FRONTEND_URL=$(az staticwebapp show \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --query "defaultHostname" -o tsv)

echo "✅ Deployment complete!"
echo ""
echo "🎉 Your SmartGarden is now live!"
echo ""
echo "Backend API: https://$APP_NAME.azurewebsites.net"
echo "Frontend App: https://$FRONTEND_URL"
echo "Database: $SQL_SERVER.database.windows.net"
echo ""
echo "📱 Next steps:"
echo "1. Open https://$FRONTEND_URL in your browser"
echo "2. Create an account"
echo "3. Power on your ESP32 device"
echo "4. Approve the device in the web app"
echo "5. Start monitoring your plants!"
```

---

## Option 2: Docker Deployment (Self-Hosted or Cloud) 🐳

### Using Docker Compose (Simplest)

1. **Create docker-compose.yml** (already in DEPLOYMENT_README.md)

2. **Set environment variables**

Create `.env` file:
```env
SQL_PASSWORD=YourStrong@Passw0rd123
JWT_SECRET=your-super-secret-key-minimum-32-characters-long
PERENUAL_API_KEY=your-perenual-api-key
```

3. **Deploy**

```bash
cd ~/SmartGarden_WorkInProgress

# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Your app is now running at:
# Backend: http://localhost:5000
# Frontend: http://localhost:3000
```

### Deploy Docker to Azure Container Instances

```bash
# Login to Azure Container Registry
az acr create \
  --name smartgardenacr \
  --resource-group $RESOURCE_GROUP \
  --sku Basic

# Build and push images
az acr build \
  --registry smartgardenacr \
  --image smartgarden-api:latest \
  --file MobileApp/SmartGarden/SmartGarden.Project/Dockerfile \
  .

# Create container instance
az container create \
  --resource-group $RESOURCE_GROUP \
  --name smartgarden-api \
  --image smartgardenacr.azurecr.io/smartgarden-api:latest \
  --dns-name-label smartgarden-api-unique \
  --ports 80
```

---

## Option 3: AWS Deployment 🟧

### Quick Deploy with Elastic Beanstalk

```bash
# Install EB CLI
pip install awsebcli

# Configure AWS credentials
aws configure

# Initialize and deploy
cd ~/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project
eb init smartgarden-api --platform ".NET Core" --region us-east-1
eb create smartgarden-prod

# Deploy updates
dotnet publish -c Release
eb deploy
```

---

## Cost Breakdown 💰

### Azure Free Tier (First 12 months)
- **App Service B1:** ~$13/month (750 hours free first year)
- **SQL Database Basic:** ~$5/month
- **Static Web Apps:** FREE
- **Bandwidth:** 15GB free/month
- **Total:** ~$18/month (or FREE first year)

### Production Tier
- **App Service S1:** ~$70/month
- **SQL Database S0:** ~$15/month
- **Static Web Apps:** FREE
- **Application Insights:** ~$5/month
- **Total:** ~$90/month

### Docker Self-Hosted (DigitalOcean/AWS/Azure VM)
- **2GB RAM VM:** ~$10-15/month
- **Storage:** ~$5/month
- **Total:** ~$15-20/month

---

## Post-Deployment Checklist ✅

### 1. Verify Backend API
```bash
# Check health endpoint
curl https://your-app.azurewebsites.net/health

# Check Swagger documentation
# Visit: https://your-app.azurewebsites.net/swagger
```

### 2. Test User Registration
```bash
curl -X POST https://your-app.azurewebsites.net/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

### 3. Configure CORS (if needed)

In `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://your-frontend.azurestaticapps.net")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### 4. Set Up Monitoring

```bash
# Enable Application Insights
az monitor app-insights component create \
  --app smartgarden-insights \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --application-type web

# Get instrumentation key
INSTRUMENTATION_KEY=$(az monitor app-insights component show \
  --app smartgarden-insights \
  --resource-group $RESOURCE_GROUP \
  --query "instrumentationKey" -o tsv)

# Add to app settings
az webapp config appsettings set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings ApplicationInsights__InstrumentationKey=$INSTRUMENTATION_KEY
```

### 5. Set Up Backups

```bash
# Enable database backups (automatic for Azure SQL)
# Configure retention period (7 days default)

az sql db ltr-policy set \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --database $SQL_DATABASE \
  --weekly-retention P4W \
  --monthly-retention P12M
```

---

## Troubleshooting Common Issues 🔧

### Issue 1: Database Connection Failed

**Symptoms:** API returns 500 errors, logs show "Cannot connect to database"

**Solution:**
```bash
# Check if your IP is allowed
az sql server firewall-rule list \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER

# Add your current IP
MY_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowMyNewIP \
  --start-ip-address $MY_IP \
  --end-ip-address $MY_IP
```

### Issue 2: SignalR Not Connecting

**Symptoms:** Real-time updates don't work

**Solution:**
```bash
# Ensure WebSocket is enabled
az webapp config set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --web-sockets-enabled true

# Restart the app
az webapp restart \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP
```

### Issue 3: ESP32 Cannot Register

**Symptoms:** Device shows "HTTP 401" or "Request timeout"

**Solution:**
1. Verify API URL in ESP32 config (must be HTTPS)
2. Check device firewall allows outbound HTTPS
3. Verify backend API is running
4. Check backend logs for errors

```bash
# View backend logs
az webapp log tail \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP
```

### Issue 4: Frontend Shows CORS Error

**Symptoms:** Browser console shows "CORS policy" error

**Solution:**
Update CORS in backend `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",  // Development
            "https://your-frontend.azurestaticapps.net"  // Production
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

---

## Updating Your Deployment 🔄

### Update Backend

```bash
cd ~/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project

# Rebuild and redeploy
dotnet publish SmartGarden.API/SmartGarden.API.csproj -c Release -o ./publish
cd publish && zip -r ../deploy.zip . && cd ..

az webapp deploy \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --src-path deploy.zip \
  --type zip
```

### Update Frontend

```bash
cd ~/SmartGarden_WorkInProgress/ReactNativeApp/SmartGardenApp

npm run build

az staticwebapp deploy \
  --name smartgarden-frontend \
  --resource-group $RESOURCE_GROUP \
  --app-location ./dist
```

### Update Database Schema

```bash
# Create new migration locally
cd ~/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.Data

dotnet ef migrations add YourMigrationName \
  --startup-project ../SmartGarden.API

# Apply to production database
dotnet ef database update \
  --startup-project ../SmartGarden.API \
  --connection "Server=tcp:$SQL_SERVER.database.windows.net,1433;Database=$SQL_DATABASE;User ID=$SQL_ADMIN;Password=$SQL_PASSWORD;Encrypt=true;"
```

---

## Scaling Your Deployment 📈

### Scale Up (Vertical Scaling)

```bash
# Upgrade App Service tier
az appservice plan update \
  --name smartgarden-plan \
  --resource-group $RESOURCE_GROUP \
  --sku S1  # Standard tier

# Upgrade database tier
az sql db update \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $SQL_DATABASE \
  --service-objective S1
```

### Scale Out (Horizontal Scaling)

```bash
# Add more instances
az appservice plan update \
  --name smartgarden-plan \
  --resource-group $RESOURCE_GROUP \
  --number-of-workers 3
```

---

## Security Hardening 🔒

```bash
# 1. Enable managed identity
az webapp identity assign \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP

# 2. Use Azure Key Vault for secrets
az keyvault create \
  --name smartgarden-vault \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

# 3. Store JWT secret in Key Vault
az keyvault secret set \
  --vault-name smartgarden-vault \
  --name JwtSecret \
  --value "your-super-secret-jwt-key"

# 4. Grant app access to Key Vault
PRINCIPAL_ID=$(az webapp identity show \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query principalId -o tsv)

az keyvault set-policy \
  --name smartgarden-vault \
  --object-id $PRINCIPAL_ID \
  --secret-permissions get list
```

---

## Need Help? 🆘

### Check Azure Portal
1. Go to https://portal.azure.com
2. Navigate to your resource group
3. Click on your App Service
4. Check "Log stream" for real-time logs

### Useful Commands

```bash
# View all resources
az resource list --resource-group $RESOURCE_GROUP --output table

# Check app status
az webapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query state

# Download logs
az webapp log download --name $APP_NAME --resource-group $RESOURCE_GROUP

# Delete everything (careful!)
az group delete --name $RESOURCE_GROUP --yes --no-wait
```

---

**🎉 Congratulations! Your SmartGarden is now running in the cloud!**

Your plants can now be monitored from anywhere in the world! 🌍🌱
