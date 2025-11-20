#!/bin/bash

# SmartGarden Backend Setup Script
# This script sets up the backend API with database migrations

set -e  # Exit on error

echo "======================================"
echo "  SmartGarden Backend Setup"
echo "======================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK is not installed${NC}"
    echo "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi

echo -e "${GREEN}✓${NC} .NET SDK found: $(dotnet --version)"
echo ""

# Step 1: Restore packages
echo "Step 1: Restoring NuGet packages..."
dotnet restore
echo -e "${GREEN}✓${NC} Packages restored"
echo ""

# Step 2: Build the solution
echo "Step 2: Building solution..."
dotnet build --no-restore
echo -e "${GREEN}✓${NC} Build successful"
echo ""

# Step 3: Check database connection
echo "Step 3: Checking database configuration..."
echo -e "${YELLOW}Please ensure your database connection string is configured in:${NC}"
echo "  SmartGarden.API/appsettings.json"
echo ""
read -p "Press Enter to continue once database is configured..."

# Step 4: Create migration for new security features
echo ""
echo "Step 4: Creating database migration for security features..."
dotnet ef migrations add AddDeviceAuthAndSecurityFeatures \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API \
  --context SmartGardenDbContext

echo -e "${GREEN}✓${NC} Migration created"
echo ""

# Step 5: Apply migrations
echo "Step 5: Applying database migrations..."
dotnet ef database update \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API \
  --context SmartGardenDbContext

echo -e "${GREEN}✓${NC} Database updated"
echo ""

# Step 6: Display next steps
echo "======================================"
echo -e "${GREEN}Setup Complete!${NC}"
echo "======================================"
echo ""
echo "To start the backend API, run:"
echo -e "${YELLOW}  cd SmartGarden.API${NC}"
echo -e "${YELLOW}  dotnet run${NC}"
echo ""
echo "The API will be available at:"
echo "  - HTTPS: https://localhost:5000"
echo "  - HTTP: http://localhost:5001"
echo "  - Swagger UI: https://localhost:5000"
echo ""
echo "Database tables created:"
echo "  ✓ Users, UserSettings"
echo "  ✓ Plants, Species, SoilTypes"
echo "  ✓ Devices, DeviceAuths, DeviceCommands"
echo "  ✓ SensorReadings, WateringLogs, WateringSchedules"
echo "  ✓ PlantThresholds, PlantHealths, PlantPhotos"
echo "  ✓ Alerts, NotificationSettings"
echo "  ✓ MaintenanceLogs, AuditLogs, SystemLogs"
echo ""
