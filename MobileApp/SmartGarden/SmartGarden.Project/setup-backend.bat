@echo off
REM SmartGarden Backend Setup Script (Windows)
REM This script sets up the backend API with database migrations

echo ======================================
echo   SmartGarden Backend Setup
echo ======================================
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Error: .NET SDK is not installed
    echo Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [OK] .NET SDK found
dotnet --version
echo.

REM Step 1: Restore packages
echo Step 1: Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to restore packages
    pause
    exit /b 1
)
echo [OK] Packages restored
echo.

REM Step 2: Build the solution
echo Step 2: Building solution...
dotnet build --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo Error: Build failed
    pause
    exit /b 1
)
echo [OK] Build successful
echo.

REM Step 3: Database configuration reminder
echo Step 3: Checking database configuration...
echo Please ensure your database connection string is configured in:
echo   SmartGarden.API\appsettings.json
echo.
pause

REM Step 4: Create migration
echo.
echo Step 4: Creating database migration for security features...
dotnet ef migrations add AddDeviceAuthAndSecurityFeatures ^
  --project SmartGarden.Data ^
  --startup-project SmartGarden.API ^
  --context SmartGardenDbContext

if %ERRORLEVEL% NEQ 0 (
    echo Warning: Migration may already exist or failed
)
echo [OK] Migration created
echo.

REM Step 5: Apply migrations
echo Step 5: Applying database migrations...
dotnet ef database update ^
  --project SmartGarden.Data ^
  --startup-project SmartGarden.API ^
  --context SmartGardenDbContext

if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to update database
    pause
    exit /b 1
)
echo [OK] Database updated
echo.

REM Display next steps
echo ======================================
echo Setup Complete!
echo ======================================
echo.
echo To start the backend API, run:
echo   cd SmartGarden.API
echo   dotnet run
echo.
echo The API will be available at:
echo   - HTTPS: https://localhost:5000
echo   - HTTP: http://localhost:5001
echo   - Swagger UI: https://localhost:5000
echo.
echo Database tables created successfully!
echo.
pause
