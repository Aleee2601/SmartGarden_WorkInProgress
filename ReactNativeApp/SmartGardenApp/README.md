# 🌱 SmartGarden React Web App

A modern, fully-integrated web application for the SmartGarden IoT plant monitoring system. Built with React 18, Vite, and Tailwind CSS, featuring **real-time SignalR updates**, **historical analytics**, **smart plant search**, and automated watering controls.

## ✨ Features

### 🔐 Authentication
- User registration and login with JWT
- Automatic token refresh
- Secure session management
- Device authentication for ESP32

### 📊 Real-Time Dashboard
- **Live sensor updates** via SignalR WebSocket
- View all plants with instant status changes
- Automatic reconnection with exponential backoff
- No polling required - push-based updates
- Real-time watering status indicators

### 🔍 Smart Plant Search
- **40,000+ plant species** database (Perenual API)
- Search by common name or scientific name
- Get optimal care requirements
- Suggested moisture thresholds
- Sunlight and watering frequency recommendations
- Debounced search (500ms) for better UX

### 📈 Historical Analytics
- **Interactive time-series charts** with Recharts
- Hourly, daily, and weekly data aggregation
- Multiple sensor metrics on one chart
- Date range selector (7, 14, 30 days)
- Toggle visibility of individual metrics
- Min/Max/Avg statistics
- Watering event overlays

### 🌿 Plant Management
- Add, edit, and delete plants
- Auto-populated care instructions from search
- Plant nickname and notes
- Device association
- Sensor calibration per plant

### 🚰 Watering Control
- **Intelligent auto-watering** based on soil moisture
- Manual watering with configurable duration
- Watering history and logs
- Real-time watering status
- Tank level monitoring

### 📱 Device Management
- ESP32 device approval workflow
- Device status monitoring
- Firmware version tracking
- Multiple devices per user

### 🎨 UI/UX
- Modern, responsive design
- Green color scheme optimized for plant care
- Smooth animations and transitions
- Loading states and error handling
- Mobile-first responsive layout

## 🚀 Quick Start

### Prerequisites

- **Node.js 18+** and npm
- **Backend API** running (.NET 10 SmartGarden.API)
- **SQL Server** database
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
   VITE_API_URL=https://localhost:5001/api
   VITE_SIGNALR_HUB_URL=https://localhost:5001/hubs/plant
   ```

3. **Start development server:**
   ```bash
   npm run dev
   ```

   The app will open at `http://localhost:5173`

### Production Build

```bash
npm run build
npm run preview
```

Build output goes to `dist/` directory.

## 📁 Project Structure

```
SmartGardenApp/
├── src/
│   ├── api/                    # API services
│   │   ├── apiClient.js        # HTTP client with interceptors
│   │   ├── authService.js      # Authentication API
│   │   ├── plantService.js     # Plant CRUD + search operations
│   │   ├── analyticsService.js # Historical analytics API
│   │   ├── sensorService.js    # Sensor data API
│   │   ├── deviceService.js    # Device management API
│   │   ├── wateringService.js  # Watering control API
│   │   └── index.js            # Services export
│   ├── components/             # React components
│   │   ├── Dashboard.jsx       # Main dashboard with SignalR
│   │   ├── PlantAnalytics.jsx  # Historical charts component
│   │   ├── AddPlantWizard.jsx  # Plant search & add wizard
│   │   └── ...                 # Other components
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
  API: {
    BASE_URL: import.meta.env.VITE_API_URL || 'https://localhost:5001/api',
    TIMEOUT: 10000
  },
  SIGNALR: {
    HUB_URL: import.meta.env.VITE_SIGNALR_HUB_URL || 'https://localhost:5001/hubs/plant',
    RECONNECT_DELAYS: [1000, 2000, 5000, 10000, 30000]
  },
  PERENUAL: {
    SEARCH_DEBOUNCE_MS: 500
  }
};
```

### Dependencies

```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "@microsoft/signalr": "^8.0.0",
    "recharts": "^2.10.0",
    "lucide-react": "^0.263.1"
  },
  "devDependencies": {
    "vite": "^5.0.0",
    "tailwindcss": "^3.3.0",
    "@vitejs/plugin-react": "^4.2.0"
  }
}
```

## 🎯 Usage

### User Registration & Login

1. **Sign Up:**
   - Click "Create new account" on login screen
   - Enter username, email, and password
   - Complete account creation
   - Redirect to dashboard

2. **Login:**
   - Enter registered email and password
   - Click "Login" to access dashboard

### Adding Plants with Smart Search

1. **Open Add Plant Wizard:**
   - Click "Add Plant" button on dashboard
   - Search wizard appears

2. **Search for Plant:**
   - Type plant name (e.g., "Monstera" or "Aloe Vera")
   - Results appear after 500ms delay
   - Click on a plant to select it

3. **Review Care Instructions:**
   - View suggested moisture threshold
   - Check sunlight requirements
   - Review watering frequency
   - Read care notes

4. **Complete Addition:**
   - Enter plant nickname
   - Add personal notes (optional)
   - Associate with ESP32 device
   - Click "Add Plant"

### Real-Time Monitoring

1. **Dashboard View:**
   - All plants displayed as cards
   - Real-time sensor readings update automatically
   - Watering status shows live
   - No page refresh needed

2. **SignalR Connection:**
   - Automatic connection on dashboard load
   - Status indicator shows connection health
   - Auto-reconnect if connection drops
   - Green indicator = Connected
   - Yellow indicator = Reconnecting
   - Red indicator = Disconnected

### Historical Analytics

1. **View Plant History:**
   - Click on plant card
   - Navigate to "Analytics" tab
   - Charts load automatically

2. **Interact with Charts:**
   - Select date range (7, 14, or 30 days)
   - Toggle metrics on/off:
     - Soil Moisture (%)
     - Air Temperature (°C)
     - Air Humidity (%)
     - Light Level (lux)
     - Air Quality (%)
   - Hover for detailed tooltips
   - View watering events as markers

3. **View Statistics:**
   - Current values for all metrics
   - Min/Max/Average calculations
   - Watering frequency
   - Last watered timestamp

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
   - Select plant to calibrate

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
   - Watch real-time watering status update

2. **Automatic Watering:**
   - ESP32 automatically checks soil moisture
   - Waters when below threshold
   - Updates broadcast to all connected clients

## 🌐 API Integration

### SignalR Real-Time Updates

```javascript
import * as signalR from '@microsoft/signalr';

// Create connection
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${ENV.SIGNALR.HUB_URL}`, {
    transport: signalR.HttpTransportType.WebSockets |
               signalR.HttpTransportType.ServerSentEvents,
    withCredentials: true
  })
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: (retryContext) => {
      return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
    }
  })
  .build();

// Listen for updates
connection.on('ReceiveUpdate', (update) => {
  console.log('Plant update:', update);
  // Update UI with new sensor data
});

// Start connection
await connection.start();
```

### Plant Search with Perenual API

```javascript
import { plantService } from './api';

// Debounced search
const searchPlants = async (query) => {
  if (!query || query.length < 2) return [];

  const results = await plantService.searchPlants(query);
  return results;
};

// Usage in component with debouncing
const [searchTerm, setSearchTerm] = useState('');
const [results, setResults] = useState([]);

useEffect(() => {
  const timer = setTimeout(async () => {
    if (searchTerm) {
      const data = await searchPlants(searchTerm);
      setResults(data);
    }
  }, 500);

  return () => clearTimeout(timer);
}, [searchTerm]);
```

### Historical Analytics

```javascript
import { analyticsService } from './api';

// Get historical data
const startDate = new Date();
startDate.setDate(startDate.getDate() - 7);
const endDate = new Date();

const data = await analyticsService.getHistoricalData(
  plantId,
  startDate,
  endDate,
  'hourly' // or 'daily', 'weekly'
);

// Get plant statistics
const stats = await analyticsService.getPlantSummary(plantId);
console.log('Avg soil moisture:', stats.averageSoilMoisture);
```

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

// Get historical readings
const history = await sensorService.getHistory(plantId, 24); // last 24 hours
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

- **React 18.2** - UI framework with hooks
- **Vite 5.0** - Lightning-fast build tool
- **Tailwind CSS 3.3** - Utility-first CSS framework
- **@microsoft/signalr 8.0** - Real-time WebSocket communication
- **Recharts 2.10** - Composable charting library
- **Lucide React** - Beautiful icon library
- **Fetch API** - HTTP client

## 📊 Component Architecture

### Component Hierarchy

```
App
├── LoginScreen
├── SignUpScreen
├── Dashboard (with SignalR)
│   ├── Header
│   ├── MenuSidebar
│   ├── PlantCardList
│   │   └── PlantCard (real-time updates)
│   └── AddPlantWizard
│       ├── PlantSearch
│       └── PlantForm
└── PlantDetailScreen
    ├── SensorReadings
    ├── PlantAnalytics (Recharts)
    │   ├── DateRangeSelector
    │   ├── MetricToggles
    │   ├── TimeSeriesChart
    │   └── StatisticsCards
    ├── AutoWateringControls
    └── DeviceInfo
```

### State Management

- **Local component state** with React hooks
- **SignalR connection** managed in Dashboard
- **JWT tokens** in localStorage
- **API client singleton** with automatic token management
- **Real-time updates** via SignalR push events

### Real-Time Data Flow

```
ESP32 Device
    ↓ (POST /api/telemetry)
TelemetryController
    ↓ (SignalR broadcast)
PlantHub (/hubs/plant)
    ↓ (WebSocket push)
Dashboard Component
    ↓ (State update)
PlantCard Re-renders
```

### Error Handling

- User-friendly error messages
- Automatic retry for failed requests
- Token refresh on 401 errors
- Network timeout handling (10 seconds)
- SignalR reconnection with exponential backoff
- Graceful degradation when WebSocket unavailable

## 🔒 Security

- **JWT Authentication:** Secure user sessions
- **Token Refresh:** Automatic token renewal
- **HTTPS Only:** All API calls over HTTPS
- **CORS Protection:** Configured in backend
- **Input Validation:** Client-side validation
- **XSS Protection:** React's built-in escaping
- **SignalR Authentication:** Credentials passed with connection

## 🐛 Troubleshooting

### Backend Connection Issues

**Problem:** "Request timeout" or "Failed to fetch"

**Solution:**
1. Ensure backend API is running
2. Check API URL in `.env` matches backend address
3. Verify CORS is configured for `http://localhost:5173`
4. Check firewall allows HTTPS connections

### SignalR Not Connecting

**Problem:** Real-time updates not working

**Solution:**
1. Verify SignalR hub URL is correct
2. Check backend has SignalR configured
3. Ensure WebSocket is not blocked by firewall/proxy
4. Check browser console for connection errors
5. Verify CORS allows SignalR connections

### Plant Search Not Working

**Problem:** No results when searching

**Solution:**
1. Check Perenual API key is configured in backend
2. Verify API key has not exceeded rate limit
3. Check network tab for API call errors
4. Ensure search term is at least 2 characters

### Analytics Not Loading

**Problem:** Charts don't display data

**Solution:**
1. Verify plant has sensor readings in database
2. Check date range includes data
3. Ensure analyticsService API calls succeed
4. Check browser console for Recharts errors

### Login Fails

**Problem:** "Login failed" error

**Solution:**
1. Verify user credentials are correct
2. Check backend database has users table
3. Ensure JWT secret is set in backend configuration
4. Check backend logs for authentication errors

## 📝 API Documentation

### Base URL
```
https://localhost:5001/api
```

### Key Endpoints

#### Authentication
- `POST /auth/login` - User login
- `POST /auth/register` - User registration
- `POST /auth/device/register` - Device registration
- `POST /auth/device/login` - Device authentication

#### Plants
- `GET /plants` - Get all user plants
- `GET /plants/{id}` - Get plant by ID
- `POST /plants` - Create new plant
- `PUT /plants/{id}` - Update plant
- `DELETE /plants/{id}` - Delete plant
- `GET /plants/search?q=monstera` - Search plants (Perenual)
- `GET /plants/search/{id}/details` - Get plant details

#### Telemetry
- `POST /telemetry` - Submit sensor readings (ESP32)

#### Analytics
- `GET /analytics/plant/{id}/historical` - Time-series data
- `GET /analytics/plant/{id}/summary` - Plant statistics

#### Sensors
- `GET /sensor/plant/{plantId}/latest` - Latest reading
- `GET /sensor/plant/{plantId}/history` - Historical readings

#### Watering
- `POST /watering/manual` - Manual watering
- `GET /watering/plant/{plantId}/logs` - Watering history

### SignalR Hub

**Hub URL:** `https://localhost:5001/hubs/plant`

**Events:**
- `ReceiveUpdate` - Broadcast when sensor data received
- `WateringStarted` - Broadcast when watering begins
- `WateringCompleted` - Broadcast when watering ends

## 🚀 Performance Optimization

### Code Splitting

```javascript
// Lazy load analytics component
const PlantAnalytics = React.lazy(() => import('./components/PlantAnalytics'));
```

### Debouncing

```javascript
// Search debounce to prevent excessive API calls
const debounce = (func, delay) => {
  let timeoutId;
  return (...args) => {
    clearTimeout(timeoutId);
    timeoutId = setTimeout(() => func(...args), delay);
  };
};
```

### SignalR Optimization

```javascript
// Only subscribe to updates when dashboard is active
useEffect(() => {
  if (isActive) {
    connection.start();
  }
  return () => connection.stop();
}, [isActive]);
```

## 🎓 For Thesis/Academic Use

This project demonstrates:
- **Full-stack IoT architecture** with React frontend
- **Real-time communication** using SignalR WebSocket
- **Data visualization** with interactive charts
- **RESTful API integration** with proper error handling
- **Modern React patterns** (hooks, context, lazy loading)
- **Production-ready code** structure
- **Secure authentication** with JWT
- **Responsive design** with TailwindCSS

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

## Related Documentation

- [Main Project Documentation](../../PROJECT_DOCUMENTATION.md)
- [API Reference](../../API_REFERENCE.md)
- [Backend API README](../../MobileApp/SmartGarden/SmartGarden.Project/SmartGarden.API/README.md)
- [User Guide](../../USER_GUIDE.md)

---

**Built with ❤️ for plant lovers and IoT enthusiasts**

**Version:** 2.0
**Last Updated:** November 2025
**Framework:** React 18 + Vite 5
