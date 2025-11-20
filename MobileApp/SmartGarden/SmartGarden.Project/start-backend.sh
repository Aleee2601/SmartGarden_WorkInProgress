#!/bin/bash

# SmartGarden Backend Start Script
# Quick start script for running the backend API

set -e  # Exit on error

echo "======================================"
echo "  Starting SmartGarden Backend API"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK is not installed${NC}"
    exit 1
fi

# Navigate to API project
cd SmartGarden.API

echo -e "${YELLOW}Starting API server...${NC}"
echo ""
echo "API will be available at:"
echo "  - HTTPS: https://localhost:5000"
echo "  - HTTP: http://localhost:5001"
echo "  - Swagger UI: https://localhost:5000"
echo ""
echo "Press Ctrl+C to stop the server"
echo ""

# Run the API
dotnet run
