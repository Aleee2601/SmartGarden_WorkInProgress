# 🌱 Bloomly - SmartGarden React Web App

A modern, fully-integrated web application for the SmartGarden IoT plant monitoring system. Built with React, Vite, and Tailwind CSS, featuring real-time sensor data, device management, and automated watering controls.

## ✨ Features

### 🔐 Authentication
- User registration and login
- JWT token-based authentication
- Automatic token refresh
- Secure session management

### 📱 Core Functionality
- **Dashboard**: View all plants with real-time status
- **Plant Management**: Add, edit, and delete plants
- **Device Approval**: Approve new ESP32 devices
- **Sensor Monitoring**: Real-time sensor data display
- **Calibration**: Multi-sensor calibration wizard with countdown timers
- **Auto-Watering**: Configure automatic watering schedules
- **Manual Control**: Trigger manual watering
- **Weekly Statistics**: Visual charts showing sensor trends

### 🎨 UI/UX
- Modern, responsive design
- Green color scheme optimized for plant care
- Smooth animations and transitions
- Loading states and error handling
- Mobile-first responsive layout

## 🚀 Quick Start

### Prerequisites

- Node.js 18+ and npm
- Backend API running (C# .NET SmartGarden.API)
- ESP32 device (optional, for full functionality)

### Installation

1. **Install dependencies:**
   ```bash
   cd ReactNativeApp/SmartGardenApp
   npm install
   ```

2. **Configure environment:**
   ```bash
   cp .env.example .env
   ```

   Edit `.env` and set your API URL:
   ```env
   REACT_APP_API_URL=https://localhost:5000/api
   ```

3. **Start development server:**
   ```bash
   npm run dev
   ```

   The app will open at `http://localhost:3000`

### Production Build

```bash
npm run build
npm run preview
```

## 📁 Project Structure

```
SmartGardenApp/
├── src/
│   ├── api/                    # API services
│   │   ├── apiClient.js        # HTTP client with interceptors
│   │   ├── authService.js      # Authentication API
│   │   ├── plantService.js     # Plant CRUD operations
│   │   ├── sensorService.js    # Sensor data API
│   │   ├── deviceService.js    # Device management API
│   │   ├── wateringService.js  # Watering control API
│   │   └── index.js            # Services export
│   ├── config/
│   │   └── env.js              # Environment configuration
│   ├── App.jsx                 # Main application component
│   ├── main.jsx                # React entry point
│   └── index.css               # Global styles (Tailwind)
├── index.html                  # HTML entry point
├── package.json                # Dependencies
├── vite.config.js              # Vite configuration
├── tailwind.config.js          # Tailwind CSS configuration
└── README.md                   # This file
```

## 🔧 Configuration

### API Endpoints

Edit `src/config/env.js` to configure API endpoints:

```javascript
const ENV = {
  API_BASE_URL: 'https://localhost:5000/api',
  ENDPOINTS: {
    AUTH: { /* ... */ },
    PLANTS: { /* ... */ },
    SENSOR: { /* ... */ },
    // ...
  }
};
```

### Sensor Configuration

Adjust sensor polling intervals and calibration settings:

```javascript
SENSORS: {
  READING_INTERVAL: 15 * 60 * 1000,  // 15 minutes
  CALIBRATION_COUNTDOWN: 10,          // seconds
}
```

## 🎯 Usage

### User Registration & Login

1. **Sign Up:**
   - Click "Create new account" on login screen
   - Enter username, email, and password
   - Complete account creation
   - Proceed to sensor calibration (optional)

2. **Login:**
   - Enter registered email and password
   - Click "Login" to access dashboard

### Plant Management

1. **View Plants:**
   - Dashboard displays all your plants
   - Click on a plant card to view details

2. **Plant Details:**
   - View real-time sensor readings
   - Monitor water tank, light, temperature, soil moisture
   - View weekly statistics chart

### Device Approval

1. **Pending Devices:**
   - New ESP32 devices appear in orange banner
   - Click "Approve" to authorize the device
   - Device can now send sensor data

2. **Device Registration (ESP32):**
   - ESP32 auto-registers on first boot
   - Sends MAC address and firmware version
   - Waits for user approval

### Sensor Calibration

1. **Access Calibration:**
   - Menu → "Calibrate sensors"
   - Or automatically after signup

2. **Calibration Process:**
   - Select sensor type (Light, Soil, Water, Temperature)
   - Follow on-screen instructions
   - Click "Calibrate" for each step
   - 10-second countdown captures sensor value
   - Complete all steps for 100% progress

3. **Sensor Types:**
   - **Light Sensor:** 5 levels (Dark → Bright Direct)
   - **Soil Moisture:** 3 levels (Dry → Saturated)
   - **Water Level:** 3 levels (Empty → Full)
   - **Temperature:** 1 level (Room Temperature)

### Watering Control

1. **Manual Watering:**
   - Go to Plant Detail screen
   - Click "Water Now" button
   - 30-second watering cycle starts

2. **Automatic Watering:**
   - Toggle "Auto Mode" ON/OFF
   - Adjust watering frequency (1-5 dots)
   - System waters automatically based on soil moisture

## 🌐 API Integration

### Authentication Flow

```javascript
import { authService } from './api';

// Login
const response = await authService.login(email, password);
// Token automatically stored in localStorage

// Get current user
const user = authService.getCurrentUser();

// Logout
await authService.logout();
```

### Fetching Sensor Data

```javascript
import { sensorService } from './api';

// Get latest reading
const data = await sensorService.getLatestReading(plantId);

// Start real-time polling
const stopPolling = sensorService.startPolling(plantId, (data) => {
  console.log('New sensor data:', data);
}, 60000); // Poll every 60 seconds

// Stop polling
stopPolling();
```

### Watering Control

```javascript
import { wateringService } from './api';

// Manual watering
await wateringService.waterManually(plantId, 30); // 30 seconds

// Configure auto-watering
await wateringService.configureAutoWatering(plantId, {
  enabled: true,
  duration: 30,
  minSoilMoisture: 30,
  intensity: 3
});
```

## 🛠️ Development

### Running Tests

```bash
npm run lint
```

### Code Formatting

```bash
npm run format
```

### Technology Stack

- **React 18.2** - UI framework
- **Vite 5.0** - Build tool & dev server
- **Tailwind CSS 3.3** - Utility-first CSS
- **Lucide React** - Icon library
- **Fetch API** - HTTP client

## 📊 Features Breakdown

### Component Hierarchy

```
App
├── LoginScreen
├── SignUpScreen
├── CalibrationScreen
│   └── CalibrationModal
├── DashboardScreen
│   ├── Header
│   ├── MenuSidebar
│   ├── PlantList
│   └── TipsSection
└── PlantDetailScreen
    ├── SensorReadings
    ├── AutoWateringControls
    └── WeeklyStatisticsChart
```

### State Management

- Local component state with React hooks
- JWT tokens in localStorage
- API client singleton with automatic token management
- Real-time sensor data polling

### Error Handling

- User-friendly error messages
- Automatic retry for failed requests
- Token refresh on 401 errors
- Network timeout handling (10 seconds)

## 🔒 Security

- **JWT Authentication:** Secure user sessions
- **Token Refresh:** Automatic token renewal
- **HTTPS Only:** All API calls over HTTPS
- **CORS Protection:** Configured in backend
- **Input Validation:** Client-side validation
- **XSS Protection:** React's built-in escaping

## 🐛 Troubleshooting

### Backend Connection Issues

**Problem:** "Request timeout" or "Failed to fetch"

**Solution:**
1. Ensure backend API is running (`dotnet run` in SmartGarden.API)
2. Check API URL in `.env` matches backend address
3. Verify CORS is configured for `http://localhost:3000`

### Login Fails

**Problem:** "Login failed" error

**Solution:**
1. Verify user credentials are correct
2. Check backend database has users table
3. Ensure UserSecret is set in backend appsettings.json

### Sensor Data Not Updating

**Problem:** Sensor readings show 0 or outdated values

**Solution:**
1. Verify ESP32 device is approved
2. Check device is online and sending data
3. Ensure plant has associated device
4. Check backend logs for sensor API errors

### Calibration Not Saving

**Problem:** Calibration data doesn't persist

**Solution:**
1. Ensure plant is selected before calibration
2. Check backend calibration API endpoint
3. Verify user has permission to update plant

## 📝 API Documentation

### Base URL
```
https://localhost:5000/api
```

### Key Endpoints

#### Authentication
- `POST /auth/login` - User login
- `POST /auth/register` - User registration
- `POST /auth/refresh-token` - Refresh JWT token
- `GET /auth/profile` - Get current user

#### Plants
- `GET /plants` - Get all plants
- `GET /plants/{id}` - Get plant by ID
- `POST /plants` - Create new plant
- `PUT /plants/{id}` - Update plant
- `DELETE /plants/{id}` - Delete plant
- `PUT /plants/{id}/calibration` - Update calibration

#### Sensors
- `GET /sensor/plant/{plantId}/latest` - Latest reading
- `GET /sensor/plant/{plantId}/history?hours=24` - History
- `GET /sensor/plant/{plantId}/statistics?days=7` - Statistics

#### Devices
- `GET /device-auth/pending` - Pending devices
- `POST /device-auth/approve` - Approve device

#### Watering
- `POST /watering/manual` - Manual watering
- `PUT /watering/plant/{plantId}/auto` - Configure auto-watering

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📄 License

MIT License - See LICENSE file for details

## 📧 Support

For issues or questions:
- Open a GitHub issue
- Check backend API logs
- Review browser console for client errors
- Verify network requests in browser DevTools

## 🎓 For Thesis/Academic Use

This project demonstrates:
- Full-stack IoT architecture
- Secure device authentication
- Real-time data visualization
- RESTful API design
- Modern web development practices
- Production-ready code structure

---

**Built with ❤️ for plant lovers and IoT enthusiasts**
