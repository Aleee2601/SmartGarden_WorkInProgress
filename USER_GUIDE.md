# SmartGarden User Guide

Welcome to **SmartGarden** - Your intelligent plant care companion! This guide will help you get started with monitoring and caring for your plants using our IoT-powered system.

## Table of Contents

1. [Getting Started](#getting-started)
2. [User Registration & Login](#user-registration--login)
3. [Adding Your First Plant](#adding-your-first-plant)
4. [Smart Plant Search](#smart-plant-search)
5. [Sensor Calibration](#sensor-calibration)
6. [Monitoring Your Plants](#monitoring-your-plants)
7. [Watering Control](#watering-control)
8. [Device Management](#device-management)
9. [Understanding Sensor Readings](#understanding-sensor-readings)
10. [Troubleshooting](#troubleshooting)

---

## Getting Started

### What You Need

1. **Backend API** - Running on your server (e.g., http://localhost:5000)
2. **React Web App** - The user interface (e.g., http://localhost:5173)
3. **ESP32 Device** - Your IoT hardware with sensors (optional for testing)
4. **SQL Server Database** - Configured and migrated

### First Launch

1. Open your web browser and navigate to the React app URL
2. You'll be greeted with the **Bloomly** login screen
3. If you don't have an account, click "Create new account"

---

## User Registration & Login

### Creating an Account

1. Click **"Create new account"** on the login screen
2. Fill in the required information:
   - **Username**: Your display name
   - **Email**: Your email address
   - **Password**: A secure password
3. Click **"Create new account"**
4. You'll be automatically logged in and redirected to the calibration screen

### Logging In

1. Enter your **email** and **password**
2. Click **"Login"**
3. You'll be redirected to your dashboard

---

## Adding Your First Plant

SmartGarden offers **two ways** to add plants:

### Method 1: Smart Plant Search (Recommended)

This is the easiest and most intelligent way to add plants!

1. **Navigate to Dashboard**
   - After logging in, you'll see your dashboard
   - If you have no plants yet, you'll see a message: "No plants yet. Add your first plant!"

2. **Click "Add Plant" or "Add Your First Plant"**
   - A beautiful wizard will appear

3. **Step 1: Search for Your Plant**
   - Type the name of your plant species (e.g., "Basil", "Tomato", "Snake Plant")
   - Wait 500ms - the search will automatically start (debounced search)
   - Browse through the search results with:
     - Plant images
     - Scientific names
     - Watering requirements
     - Sunlight needs
     - **Auto-suggested moisture thresholds**

4. **Select Your Plant**
   - Click on the plant card that matches your plant
   - The system will auto-fill all the smart defaults

5. **Step 2: Customize Details**
   - **Nickname**: Give your plant a personal name (e.g., "My Kitchen Basil")
   - **Room/Location**: Where is the plant located? (e.g., "Kitchen", "Balcony")
   - **Moisture Threshold**: Adjust the auto-suggested value using the slider (0-100%)
   - **Outdoor/Indoor**: Check the box if this is an outdoor plant

6. **Create Your Plant**
   - Click **"Create Plant"**
   - Your plant will be added to your collection
   - You'll be redirected back to the dashboard

### Method 2: Manual Entry (Legacy)

*Note: This method requires using API endpoints directly. We recommend using the Smart Plant Search wizard instead.*

---

## Smart Plant Search

### How It Works

The Smart Plant Search feature uses the **Perenual Plant Database API** to provide:

- **40,000+ plant species** from around the world
- **Automatic moisture threshold suggestions** based on watering requirements
- **Plant images** for visual identification
- **Care information** including sunlight and watering needs

### Understanding Auto-Suggested Thresholds

The system intelligently maps watering descriptions to numeric thresholds:

| Watering Description | Suggested Moisture Threshold |
|---------------------|------------------------------|
| Frequent / Regular  | 60% |
| Average / Moderate  | 40% |
| Minimal / Drought-tolerant | 20% |
| Default (unknown)   | 40% |

**Example:**
- **Basil** (frequent watering) → 60% threshold
- **Snake Plant** (minimal watering) → 20% threshold
- **Tomato** (average watering) → 40% threshold

### Customizing Thresholds

You can always adjust the auto-suggested thresholds:

1. Use the **number input** to type a specific value (0-100)
2. Use the **slider** to visually adjust the threshold
3. The value will update in real-time

---

## Sensor Calibration

**Sensor calibration is crucial for accurate readings!** You'll be prompted to calibrate sensors after registration.

### Why Calibrate?

Different environments have different baseline readings. Calibration ensures:
- Accurate soil moisture readings
- Correct light level measurements
- Precise water level detection
- Reliable temperature readings

### Calibration Process

1. **Choose a Sensor Type**
   - Light Sensor
   - Soil Moisture
   - Water Level
   - Temperature

2. **Follow the Instructions**
   - Each sensor has multiple calibration points
   - Read the instruction carefully (e.g., "Cover the sensor completely")

3. **Start Calibration**
   - Click **"Calibrate"** on the step
   - Follow the on-screen instruction
   - Click **"Start Calibration"**
   - Wait for the countdown (default: 5 seconds)
   - The system will capture the sensor reading

4. **Complete All Steps**
   - Repeat for all calibration points
   - The progress bar shows your completion percentage

5. **Save and Finish**
   - Once all steps are calibrated, click **"Complete Setup"**
   - You can also click **"Skip for Now"** and calibrate later

### Recalibrating

You can recalibrate sensors at any time:

1. Open the **Menu** (hamburger icon on dashboard)
2. Click **"Calibrate sensors"**
3. Follow the calibration process again

---

## Monitoring Your Plants

### Dashboard Overview

Your dashboard displays:

- **User Profile**: Your username and avatar
- **Pending Devices**: Devices waiting for approval
- **My Plants**: All your registered plants
- **Tips & Tricks**: Helpful plant care advice

### Plant Cards

Each plant card shows:
- Plant emoji/icon
- Plant name (nickname)
- Species name
- Location (Indoor/Outdoor)

Click on a plant card to view **detailed information**.

---

## Plant Detail Screen

### Real-Time Sensor Readings

The plant detail screen shows **4 key metrics**:

1. **Water Tank Level** (%):
   - Shows the water reservoir level
   - Warns when level is below 20%

2. **Light Level** (lux):
   - Measures ambient light
   - Helps you optimize plant placement

3. **Temperature** (°C):
   - Air temperature around the plant
   - Ensures optimal growing conditions

4. **Soil Moisture** (%):
   - Most important metric for watering
   - Compared against your threshold settings

### Live Updates

Sensor readings update **automatically** using polling:
- Default polling interval: every 5 seconds
- No need to refresh the page
- Real-time data visualization

---

## Watering Control

### Manual Watering

1. Click **"Water Now"** button on the plant detail screen
2. The system will activate the water pump for **30 seconds**
3. A confirmation message will appear
4. Monitor the soil moisture reading to verify watering

### Automatic Watering

SmartGarden can water your plants automatically based on soil moisture levels!

#### Enabling Auto-Watering

1. Go to **Plant Detail Screen**
2. Find the **"Automatic Watering"** section
3. Toggle **"Auto Mode"** to **ON**

#### Configuring Watering Frequency

1. Adjust the **"Watering Frequency"** slider (1-5 dots)
   - 1 dot: Minimal watering
   - 5 dots: Maximum watering frequency
2. The system will check soil moisture levels and water when needed

#### How Auto-Watering Works

The system uses your **moisture threshold** settings:
- When soil moisture drops **below** `minMoistureThreshold`: Water activates
- When soil moisture reaches `maxMoistureThreshold`: Water stops
- Default watering duration: 30 seconds

**Example:**
- Min threshold: 30%
- Max threshold: 70%
- Soil moisture: 25% → System waters the plant
- Soil moisture: 35% → Watering in progress
- Soil moisture: 70% → Watering stops

---

## Device Management

### Registering an ESP32 Device

Your ESP32 device must be registered and approved before use:

1. **Flash the ESP32 Firmware**
   - Upload the firmware from `/ESP32_SmartGarden/`
   - Configure WiFi credentials in the code

2. **Boot the Device**
   - The device will automatically register with the backend
   - Status: **Pending Approval**

3. **Approve the Device**
   - Log into the web app
   - You'll see an orange banner: "X device(s) pending approval"
   - Click **"Approve"**
   - Device status: **Approved**

4. **Assign Device to Plant**
   - Edit your plant settings
   - Select the approved device from the dropdown
   - Save changes

### Device Status Indicators

- **Pending**: Device waiting for approval (orange banner)
- **Approved**: Device ready to use (no banner)
- **Active**: Device actively sending sensor data

---

## Understanding Sensor Readings

### Soil Moisture (%)

- **0-20%**: Very dry soil - Water immediately!
- **20-40%**: Dry soil - Consider watering
- **40-60%**: Ideal moisture for most plants
- **60-80%**: Moist soil - Good for water-loving plants
- **80-100%**: Saturated soil - Risk of overwatering

**Recommended Thresholds:**
- Succulents: 20-30%
- Herbs (Basil, Mint): 50-70%
- Vegetables (Tomato): 40-60%
- Tropical plants: 60-80%

### Light Level (lux)

- **0-100 lux**: Dark / Night
- **100-500 lux**: Low light (North-facing window)
- **500-1000 lux**: Medium indirect light
- **1000-5000 lux**: Bright indirect light
- **5000+ lux**: Direct sunlight

**Recommended Levels:**
- Low-light plants (Snake Plant, Pothos): 100-500 lux
- Medium-light plants (Philodendron): 500-1000 lux
- High-light plants (Basil, Tomato): 1000-5000 lux
- Full-sun plants (Succulents): 5000+ lux

### Temperature (°C)

- **Below 15°C**: Too cold for most plants
- **15-25°C**: Ideal range for most houseplants
- **25-30°C**: Warm (good for tropical plants)
- **Above 30°C**: Too hot - consider moving plant

### Water Tank Level (%)

- **0-20%**: **WARNING** - Refill immediately!
- **20-50%**: Low - Refill soon
- **50-80%**: Good level
- **80-100%**: Full tank

---

## Weekly Statistics

The plant detail screen shows a **weekly chart** with color-coded bars:

- **Green**: Light levels
- **Blue**: Water levels
- **Orange**: Temperature
- **Amber**: Soil moisture

Use this chart to:
- Identify trends in plant health
- Spot abnormal patterns
- Optimize watering schedules
- Track environmental changes

---

## Troubleshooting

### "No plants yet" Message

**Solution:** Click the "Add Your First Plant" button and use the Smart Plant Search wizard.

### "Failed to search plants" Error

**Possible causes:**
1. Backend API is not running
2. Perenual API key not configured
3. Network connectivity issue

**Solution:**
1. Ensure backend API is running (check http://localhost:5000/swagger)
2. Verify `appsettings.json` has Perenual API configuration
3. Check browser console for detailed error messages

### "X device(s) pending approval" Banner Won't Disappear

**Solution:**
1. Click the "Approve" button
2. Refresh the page
3. If issue persists, check backend logs for device approval errors

### Sensor Readings Show 0 or Null

**Possible causes:**
1. Device not connected
2. Sensors not calibrated
3. Device not assigned to plant

**Solution:**
1. Check device WiFi connection
2. Complete sensor calibration process
3. Assign device to plant in plant settings
4. Wait 5-10 seconds for first reading

### Auto-Watering Not Working

**Possible causes:**
1. Auto-watering not enabled
2. Moisture threshold not configured
3. Water pump hardware issue
4. Water tank empty

**Solution:**
1. Enable auto-watering in plant detail screen
2. Set moisture thresholds (min < max)
3. Check hardware connections (pump, relay)
4. Refill water tank

### Plant Search Returns No Results

**Possible causes:**
1. Misspelled plant name
2. Plant not in Perenual database
3. Network timeout

**Solution:**
1. Try different search terms (common name vs scientific name)
2. Use broader search terms (e.g., "basil" instead of "sweet basil")
3. Check internet connection

---

## Tips & Tricks

### Best Practices

1. **Calibrate Sensors First**
   - Always calibrate before trusting readings
   - Recalibrate every 2-3 months

2. **Start with Auto-Suggested Thresholds**
   - The Smart Plant Search provides excellent defaults
   - Adjust based on your observations

3. **Monitor Trends, Not Just Values**
   - Use weekly statistics to identify patterns
   - Sudden changes indicate problems

4. **Keep Water Tank Full**
   - Refill when level drops below 50%
   - Set reminders to check weekly

5. **Test Manual Watering First**
   - Verify watering system works before enabling auto-mode
   - Observe how long it takes to water effectively

### Plant Care Tips

The dashboard shows helpful tips:
- **Watering Rule**: Water when top 2 inches of soil is dry
- **Light Check**: Most plants need 6-8 hours of light
- **Low Light Champions**: Snake plants thrive in low light
- **Finger Test**: Stick finger in soil to check moisture

### Pro Tips

1. **Group Plants by Water Needs**
   - Succulents together (low water)
   - Herbs together (high water)
   - Makes management easier

2. **Adjust Thresholds Seasonally**
   - Winter: Lower thresholds (slower growth)
   - Summer: Higher thresholds (faster evaporation)

3. **Use Location Names Wisely**
   - "Kitchen Window" is better than "Kitchen"
   - Helps remember which plant is which

4. **Check Calibration After Moving Devices**
   - Different locations may need recalibration
   - Especially important for light sensors

---

## Advanced Features

### API Integration

SmartGarden provides a full REST API for advanced users:

**Base URL:** `http://localhost:5000/api`

**Key Endpoints:**
- `POST /auth/register` - Register new user
- `POST /auth/login` - User login
- `GET /plant/search?q={query}` - Smart plant search
- `POST /plant/from-search` - Create plant from search
- `GET /plant/{id}` - Get plant details
- `PUT /plant/{id}` - Update plant
- `GET /sensor/{plantId}/latest` - Latest sensor reading
- `POST /watering/{plantId}/manual` - Manual watering
- `PUT /watering/{plantId}/auto` - Configure auto-watering

**Documentation:** Available at `http://localhost:5000/swagger`

### Custom Sensor Integration

Want to add custom sensors?

1. Modify ESP32 firmware in `/ESP32_SmartGarden/`
2. Update `SensorReading` model in backend
3. Update frontend to display new sensor data

---

## Support & Feedback

### Getting Help

1. Check this user guide first
2. Review the [TESTING_GUIDE.md](./TESTING_GUIDE.md) for setup issues
3. Check the [BACKEND_README.md](./BACKEND_README.md) for API documentation
4. Open an issue on GitHub

### Contributing

SmartGarden is open source! Contributions welcome:
- Bug reports
- Feature requests
- Code improvements
- Documentation updates

---

## System Requirements

### Backend
- .NET 10.0 SDK
- SQL Server 2019+
- Windows, Linux, or macOS

### Frontend
- Node.js 18+
- Modern web browser (Chrome, Firefox, Safari, Edge)

### Hardware (Optional)
- ESP32 DevKit
- DHT22 temperature/humidity sensor
- Soil moisture sensor
- Light sensor (LDR or BH1750)
- Water level sensor
- Water pump + relay module
- Power supply (5V/12V depending on pump)

---

## Version Information

- **SmartGarden Version**: 2.0
- **Backend Framework**: .NET 10.0
- **Frontend Framework**: React 18
- **Database**: SQL Server
- **IoT Platform**: ESP32 Arduino

---

**Happy Gardening!** 🌱

For more information, visit the project repository or contact support.
