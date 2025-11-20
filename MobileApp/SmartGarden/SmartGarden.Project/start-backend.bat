@echo off
REM SmartGarden Backend Start Script (Windows)
REM Quick start script for running the backend API

echo ======================================
echo   Starting SmartGarden Backend API
echo ======================================
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Error: .NET SDK is not installed
    pause
    exit /b 1
)

REM Navigate to API project
cd SmartGarden.API

echo Starting API server...
echo.
echo API will be available at:
echo   - HTTPS: https://localhost:5000
echo   - HTTP: http://localhost:5001
echo   - Swagger UI: https://localhost:5000
echo.
echo Press Ctrl+C to stop the server
echo.

REM Run the API
dotnet run
