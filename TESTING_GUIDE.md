# 🧪 SmartGarden Testing Guide

Complete guide to test the SmartGarden IoT system from scratch.

---

## 📋 Prerequisites

Before testing, ensure you have:

- ✅ **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- ✅ **Node.js 18+** and npm - [Download](https://nodejs.org/)
- ✅ **SQL Server** (LocalDB/Express) - [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
- ✅ **Git** (already installed if you cloned the repo)
- ✅ **A web browser** (Chrome, Firefox, Edge, Safari)

**Optional:**
- ESP32 device with sensors (for full IoT testing)
- Postman or cURL (for API testing)

---

## 🚀 Part 1: Backend Setup & Testing

### **Step 1: Navigate to Backend**

```bash
cd /home/user/SmartGarden_WorkInProgress/MobileApp/SmartGarden/SmartGarden.Project
```

### **Step 2: Check Prerequisites**

```bash
# Check .NET version
dotnet --version
# Should show: 8.0.x or higher

# Check SQL Server (Windows)
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Or use SQL Server Express
sqlcmd -S localhost\SQLEXPRESS -E -Q "SELECT @@VERSION"
```

### **Step 3: Configure Database**

Edit `SmartGarden.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartGardenDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**For SQL Server Express:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SmartGardenDB;Integrated Security=true;TrustServerCertificate=true;"
```

**For LocalDB (Windows):**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartGardenDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### **Step 4: Run Automated Setup**

**Linux/macOS:**
```bash
chmod +x setup-backend.sh
./setup-backend.sh
```

**Windows:**
```cmd
setup-backend.bat
```

**Manual Setup (if scripts fail):**
```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Create migration
dotnet ef migrations add InitialCreate \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API

# Apply migration (creates database)
dotnet ef database update \
  --project SmartGarden.Data \
  --startup-project SmartGarden.API
```

### **Step 5: Start Backend**

```bash
cd SmartGarden.API
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5000
      Now listening on: http://localhost:5001
Application started. Press Ctrl+C to shut down.
```

### **Step 6: Test Backend API**

**Open your browser and go to:**
```
https://localhost:5000
```

You should see the **Swagger UI** with all API endpoints.

**Test the health endpoint:**
```bash
curl -k https://localhost:5000/api/home
```

**Expected response:**
```json
{
  "message": "SmartGarden API is running",
  "version": "1.0.0"
}
```

✅ **Backend is working!** Keep this terminal running.

---

## 🌐 Part 2: React Web App Setup & Testing

### **Step 1: Open New Terminal**

Open a **second terminal window** (keep backend running in first).

### **Step 2: Navigate to React App**

```bash
cd /home/user/SmartGarden_WorkInProgress/ReactNativeApp/SmartGardenApp
```

### **Step 3: Install Dependencies**

```bash
npm install
```

This will install React, Vite, Tailwind CSS, and all dependencies (~1-2 minutes).

### **Step 4: Configure API URL**

Check if `.env` file exists:
```bash
ls -la | grep .env
```

If not, create it:
```bash
cat > .env << EOF
VITE_API_URL=https://localhost:5000/api
NODE_ENV=development
EOF
```

### **Step 5: Start React App**

```bash
npm run dev
```

**Expected Output:**
```
  VITE v5.0.8  ready in 500 ms

  ➜  Local:   http://localhost:3000/
  ➜  Network: use --host to expose
  ➜  press h to show help
```

### **Step 6: Open Web App**

Open your browser and navigate to:
```
http://localhost:3000
```

You should see the **SmartGarden Login Screen**! 🎉

✅ **React app is running!**

---

## 🧪 Part 3: End-to-End Testing

Now let's test the complete workflow:

### **Test 1: User Registration**

1. **Click "Sign Up"** on the login screen
2. **Fill in the form:**
   - Email: `test@example.com`
   - Password: `Test123!`
   - Username: `TestUser`
3. **Click "Sign Up"**

**Expected:** You should be redirected to the **Calibration Screen**.

**If it fails:**
- Check browser console (F12) for errors
- Check backend terminal for API errors
- Verify database connection

### **Test 2: Skip Calibration (For Now)**

Since you don't have ESP32 hardware yet:
- Click through the calibration steps quickly
- Or refresh the page to skip to dashboard

### **Test 3: View Dashboard**

You should see:
- ✅ Welcome message with your username
- ✅ Empty plant list (you haven't added plants yet)
- ✅ "Add Plant" button
- ✅ Menu icon (hamburger menu)

### **Test 4: Add a Plant**

1. **Click "Add Plant"**
2. **Fill in details:**
   - Name: `My Test Plant`
   - Nickname: `Basil`
   - Room: `Kitchen`
   - Species: Select any
   - Soil Type: Select any
3. **Click "Save"**

**Expected:** Plant card appears on dashboard with "No Data" status.

### **Test 5: View Plant Details**

1. **Click on your plant card**
2. You should see:
   - Plant name and details
   - Sensor readings (all showing "No Data" - that's normal without ESP32)
   - "Water Now" button
   - Auto-watering toggle
   - Weekly statistics (empty chart)

### **Test 6: Manual Watering (Simulated)**

1. **Click "Water Now"** button
2. **Expected:** Button shows "Watering..." for a moment
3. **Check backend terminal** - you should see a POST request to `/api/watering/manual`

**This creates a watering command in the database** that an ESP32 would pick up.

### **Test 7: Auto-Watering Toggle**

1. **Toggle "Auto-Watering" ON**
2. **Adjust frequency slider** (1-7 days)
3. **Expected:** Settings are saved (check for success message)

### **Test 8: User Logout**

1. **Click menu icon** (top left)
2. **Click "Logout"**
3. **Expected:** Redirected to login screen

### **Test 9: User Login**

1. **Enter credentials:**
   - Email: `test@example.com`
   - Password: `Test123!`
2. **Click "Login"**
3. **Expected:** Redirected to dashboard with your plant

✅ **All basic functionality works!**

---

## 🔧 Part 4: Testing Without ESP32 Hardware

You can test the full system without physical hardware by simulating an ESP32 device:

### **Option 1: Manual API Testing with Postman/cURL**

#### **1. Register a Device**

```bash
curl -k -X POST https://localhost:5000/api/device-auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "macAddress": "AA:BB:CC:DD:EE:FF",
    "model": "ESP32-SmartGarden-v1",
    "firmwareVersion": "1.0.0"
  }'
```

**Save the response** (contains `deviceToken`, `apiKey`, `deviceId`).

#### **2. Approve Device via Web App**

1. Go to dashboard
2. Look for "Pending Devices" notification
3. Click to approve the device

#### **3. Send Simulated Sensor Data**

Replace `YOUR_DEVICE_TOKEN` with token from step 1:

```bash
curl -k -X POST https://localhost:5000/api/sensor \
  -H "Authorization: Bearer YOUR_DEVICE_TOKEN" \
  -H "X-Device-ID: 1" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 1,
    "plantId": 1,
    "soilMoisture": 45.5,
    "airTemperature": 22.5,
    "airHumidity": 55.0,
    "lightLevel": 320.0,
    "waterLevel": 75.0,
    "timestamp": 1234567890,
    "signature": "dummy-signature-for-testing"
  }'
```

**Note:** The signature will fail validation, but you can temporarily disable it in the backend for testing.

#### **4. Check Web App**

Refresh your plant detail page - you should see the sensor data!

---

### **Option 2: Use Swagger UI**

1. **Open Swagger:** https://localhost:5000
2. **Register a user** via `/api/auth/register`
3. **Login** via `/api/auth/login` - copy the JWT token
4. **Click "Authorize"** button (top right)
5. **Paste token:** `Bearer YOUR_JWT_TOKEN`
6. **Test all endpoints interactively!**

---

## 🗄️ Part 5: Database Verification

Check if data is being stored correctly:

### **Connect to Database**

**Windows (SQL Server):**
```cmd
sqlcmd -S localhost -d SmartGardenDB -E
```

**Linux/macOS (via Docker):**
```bash
# If you're using SQL Server in Docker
docker exec -it sql_server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourPassword
```

### **Run Queries**

```sql
-- Check users
SELECT UserId, Email, Username FROM Users;

-- Check plants
SELECT PlantId, Name, Nickname FROM Plants;

-- Check devices
SELECT DeviceId, MacAddress, Model FROM Devices;

-- Check sensor readings
SELECT TOP 10 * FROM SensorReadings ORDER BY ReadingId DESC;

-- Check watering logs
SELECT * FROM WateringLogs ORDER BY LogId DESC;

-- Exit
EXIT
```

---

## 🐛 Troubleshooting

### **Problem: Backend won't start**

**Error: "Connection to database failed"**
```bash
# Solution 1: Check SQL Server is running
# Windows:
sc query MSSQLSERVER

# Solution 2: Update connection string
# Edit appsettings.json with correct server name
```

**Error: "Port 5000 already in use"**
```bash
# Solution: Change port in launchSettings.json
# Or kill process using port:
# Linux/macOS:
lsof -ti:5000 | xargs kill -9
# Windows:
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

---

### **Problem: React app won't start**

**Error: "node: command not found"**
```bash
# Install Node.js from nodejs.org
```

**Error: "Cannot connect to API"**
```bash
# Solution 1: Check backend is running
curl -k https://localhost:5000/api/home

# Solution 2: Check CORS settings in backend appsettings.json
# Ensure "http://localhost:3000" is in AllowedOrigins
```

**Error: "CORS policy blocked"**
```bash
# Add to backend appsettings.json:
"Cors": {
  "AllowedOrigins": [
    "http://localhost:3000"
  ]
}
```

---

### **Problem: Login fails**

**Check:**
1. User was created successfully (check database)
2. Password meets requirements (8+ chars, uppercase, number)
3. JWT secrets are configured in appsettings.json
4. Check browser console for error details (F12)

---

### **Problem: Sensor data not showing**

**Check:**
1. Device is registered and approved
2. Device token is valid
3. Sensor data was posted successfully (check backend logs)
4. Plant ID matches your plant
5. Refresh the page / check polling is working

---

## 📊 Expected Test Results

After completing all tests, you should have:

| Test | Status | Evidence |
|------|--------|----------|
| Backend running | ✅ | Swagger UI loads at https://localhost:5000 |
| Database created | ✅ | 18 tables exist in SmartGardenDB |
| User registration | ✅ | Can create account |
| User login | ✅ | Can login and see dashboard |
| Create plant | ✅ | Plant appears in dashboard |
| View plant details | ✅ | Can see plant detail page |
| Manual watering | ✅ | Watering command created |
| Auto-watering config | ✅ | Settings saved |
| Device registration | ✅ | Device appears in pending list |
| Sensor data post | ✅ | Data stored in database |
| Data visualization | ✅ | Sensor readings show in UI |

---

## 🚀 Next Steps

Once basic testing works:

1. **Deploy Backend** - Azure, AWS, or your own server
2. **Deploy React App** - Netlify, Vercel, or hosting service
3. **Get ESP32 Hardware** - Flash firmware and connect sensors
4. **Test Full System** - Real sensor data → cloud → web app
5. **Monitor & Debug** - Check logs, fix issues, optimize

---

## 📚 Additional Resources

- **Backend API Docs:** `BACKEND_README.md`
- **React App Docs:** `REACT_APP_INTEGRATION_SUMMARY.md`
- **ESP32 Firmware:** `FirmWare/SecureESP32/README.md`
- **System Overview:** `COMPLETE_SYSTEM_SUMMARY.md`
- **Implementation Guide:** `IMPLEMENTATION_GUIDE.md`

---

## 💡 Testing Tips

1. **Use two browsers** - one for web app, one for Swagger UI
2. **Keep terminals visible** - watch for API errors in real-time
3. **Check browser console** - F12 to see JavaScript errors
4. **Use Postman** - easier than cURL for complex API testing
5. **Check database** - verify data is being stored correctly
6. **Test incrementally** - fix issues before moving to next test

---

## ✅ Quick Checklist

Before asking for help, verify:

- [ ] Backend is running (Swagger UI loads)
- [ ] React app is running (http://localhost:3000 loads)
- [ ] Database is connected (no connection errors)
- [ ] User can register and login
- [ ] Browser console shows no errors
- [ ] Backend terminal shows no errors
- [ ] CORS is configured correctly
- [ ] .env file exists with correct API URL

---

**Happy Testing! 🧪🌱**

If you encounter issues not covered here, check the troubleshooting section or review the detailed documentation in each component's README.
