# SmartGarden Mobile App (.NET MAUI)

A cross-platform mobile application for managing your SmartGarden IoT plant monitoring and watering system.

## Features

### 🌱 Plant Management
- View all your plants in a beautiful dashboard
- Real-time soil moisture and temperature monitoring
- Quick water button for manual watering
- Plant details with complete sensor history
- Add, edit, and delete plants

### 📱 Device Management
- Register and manage multiple ESP32 devices
- Online/offline status tracking
- Battery and signal strength monitoring
- Device heartbeat monitoring
- Link devices to specific plants

### 🔔 Alert System
- Real-time alerts for plant conditions
- Severity levels (Info, Warning, Critical)
- Unread alert counter
- Mark as read, dismiss, or resolve alerts
- Swipe gestures for quick actions
- Filter by severity

### 🔐 Authentication
- Secure user registration and login
- JWT token-based authentication
- Secure storage of credentials
- Auto-login on app start

## Architecture

### Technology Stack
- **.NET MAUI** - Cross-platform framework (Android, iOS, Windows, macOS)
- **C# 12** - Programming language
- **MVVM Pattern** - Clean separation of concerns
- **CommunityToolkit.Mvvm** - Modern MVVM helpers
- **Dependency Injection** - Built-in DI container

### Project Structure

```
SmartGarden.App/
├── Constants/
│   └── ApiConstants.cs          # API endpoints configuration
├── Converters/
│   ├── InvertedBoolConverter.cs
│   └── StringIsNotNullOrEmptyConverter.cs
├── Helpers/
│   └── SecureStorageHelper.cs   # Secure token storage
├── Services/
│   ├── BaseApiService.cs        # Base HTTP client
│   ├── IAuthService.cs / AuthService.cs
│   ├── IPlantService.cs / PlantService.cs
│   ├── IDeviceService.cs / DeviceService.cs
│   ├── IAlertService.cs / AlertService.cs
│   ├── ISensorService.cs / SensorService.cs
│   └── IWateringService.cs / WateringService.cs
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── PlantsViewModel.cs
│   ├── DevicesViewModel.cs
│   └── AlertsViewModel.cs
├── Views/
│   ├── LoginPage.xaml
│   ├── RegisterPage.xaml
│   ├── PlantsPage.xaml
│   ├── DevicesPage.xaml
│   └── AlertsPage.xaml
├── App.xaml                     # App resources and converters
├── AppShell.xaml                # Navigation structure
└── MauiProgram.cs               # Dependency injection setup
```

## Setup Instructions

### Prerequisites
- Visual Studio 2022 (v17.8+) or VS Code with .NET MAUI extension
- .NET 8 or .NET 9 SDK
- Android SDK (for Android development)
- Xcode (for iOS development - macOS only)

### 1. Configure API URL

Open `Constants/ApiConstants.cs` and update the `BaseUrl`:

```csharp
// For Android Emulator
public const string BaseUrl = "http://10.0.2.2:5000";

// For iOS Simulator
public const string BaseUrl = "http://localhost:5000";

// For Physical Device (replace with your computer's IP)
public const string BaseUrl = "http://192.168.1.100:5000";
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run on Android

```bash
# Android Emulator
dotnet build -t:Run -f net9.0-android

# Or use Visual Studio's Android Emulator picker
```

### 5. Run on iOS (macOS only)

```bash
# iOS Simulator
dotnet build -t:Run -f net9.0-ios

# Or use Visual Studio's iOS Simulator picker
```

### 6. Run on Windows

```bash
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

## API Configuration

The app connects to the SmartGarden backend API. Ensure your backend is running before launching the app.

### Android Emulator Network
- Use `10.0.2.2` to access `localhost` on the host machine
- Example: `http://10.0.2.2:5000`

### iOS Simulator Network
- Use `localhost` directly
- Example: `http://localhost:5000`

### Physical Device Network
- Use your computer's local IP address
- Find IP: `ipconfig` (Windows) or `ifconfig` (Mac/Linux)
- Example: `http://192.168.1.100:5000`
- **Important**: Device and computer must be on the same WiFi network

## User Guide

### First Time Setup

1. **Launch the App**
   - App opens to Login screen

2. **Register an Account**
   - Tap "Register" link
   - Enter your name, email, and password
   - Tap "Register" button

3. **Login**
   - Enter your email and password
   - Tap "Login" button
   - App navigates to Plants dashboard

### Managing Plants

**View Plants:**
- Plants tab shows all your plants
- Each card displays:
  - Plant nickname
  - Room/location
  - Current soil moisture
  - Current temperature

**Water a Plant:**
- Tap the blue "💧 Water" button on any plant card
- Watering command is sent to ESP32 device
- Success message confirms watering

**Add a Plant:**
- Tap the "+" button in the toolbar
- (Note: Add Plant page needs to be implemented)

**Refresh Data:**
- Pull down on the plants list to refresh

### Managing Devices

**View Devices:**
- Tap "Devices" tab
- Shows all registered ESP32 devices
- Online/Offline status with colored indicators
- Battery level and signal strength

**Refresh Devices:**
- Pull down on the devices list to refresh

### Managing Alerts

**View Alerts:**
- Tap "Alerts" tab
- Shows all alerts sorted by most recent
- Unread count displayed at top

**Mark Alert as Read:**
- Tap any alert to mark it as read

**Dismiss Alert:**
- Swipe left on an alert
- Tap "Dismiss" button

**Severity Indicators:**
- ℹ️ INFO - Gray background
- ⚠️ WARNING - Orange background
- 🚨 CRITICAL - Red background

### Logout

- Go to Plants tab
- Tap "Logout" in the toolbar
- Returns to Login screen

## Development

### Adding a New Page

1. Create ViewModel in `ViewModels/`
```csharp
public partial class MyViewModel : BaseViewModel
{
    public MyViewModel()
    {
        Title = "My Page";
    }
}
```

2. Create View in `Views/`
```xaml
<ContentPage ...
             x:DataType="viewmodel:MyViewModel">
    ...
</ContentPage>
```

3. Register in `MauiProgram.cs`
```csharp
builder.Services.AddTransient<MyViewModel>();
builder.Services.AddTransient<MyPage>();
```

4. Add route to `AppShell.xaml`
```xaml
<ShellContent Route="MyPage"
              ContentTemplate="{DataTemplate views:MyPage}" />
```

### Adding a New Service

1. Create interface in `Services/`
```csharp
public interface IMyService
{
    Task<Data> GetDataAsync();
}
```

2. Create implementation
```csharp
public class MyService : BaseApiService, IMyService
{
    public MyService(HttpClient httpClient) : base(httpClient) { }

    public async Task<Data> GetDataAsync()
    {
        return await GetAsync<Data>("api/endpoint");
    }
}
```

3. Register in `MauiProgram.cs`
```csharp
builder.Services.AddSingleton<IMyService, MyService>();
```

## Troubleshooting

### Cannot Connect to API

**Problem:** App shows connection errors

**Solutions:**
- Verify backend API is running
- Check API URL in `ApiConstants.cs`
- For Android Emulator, use `10.0.2.2` instead of `localhost`
- For physical device, use computer's IP address
- Ensure device and computer are on same WiFi network
- Check firewall settings

### Build Errors

**Problem:** NuGet package restore fails

**Solution:**
```bash
dotnet nuget locals all --clear
dotnet restore
dotnet build
```

### Android Emulator Issues

**Problem:** Emulator won't start

**Solutions:**
- Update Android SDK in Visual Studio
- Create new emulator device
- Enable hardware acceleration (HAXM/WHPX)

### iOS Simulator Issues (Mac Only)

**Problem:** Simulator won't launch

**Solutions:**
- Update Xcode from App Store
- Run `sudo xcode-select --reset`
- Restart Mac

## Dependencies

- **Microsoft.Maui.Controls** - MAUI framework
- **CommunityToolkit.Mvvm** (8.3.2) - MVVM helpers
- **CommunityToolkit.Maui** (9.1.1) - Additional controls
- **System.Net.Http.Json** (9.0.0) - JSON serialization
- **SmartGarden.Core** - Shared DTOs and models from backend

## API Endpoints Used

### Authentication
- POST `/api/auth/register` - Register new user
- POST `/api/auth/login` - Login user

### Plants
- GET `/api/plant` - Get all plants
- GET `/api/plant/{id}` - Get plant by ID
- POST `/api/plant` - Create plant
- DELETE `/api/plant/{id}` - Delete plant

### Devices
- GET `/api/device/user/{userId}` - Get user devices
- GET `/api/device/{id}` - Get device by ID
- POST `/api/device` - Register device
- PUT `/api/device/{id}` - Update device
- DELETE `/api/device/{id}` - Delete device
- GET `/api/device/offline` - Get offline devices

### Alerts
- GET `/api/alert/user/{userId}` - Get user alerts
- GET `/api/alert/user/{userId}/unread` - Get unread alerts
- GET `/api/alert/user/{userId}/count` - Get unread count
- PUT `/api/alert/{id}/read` - Mark as read
- PUT `/api/alert/{id}/dismiss` - Dismiss alert
- PUT `/api/alert/{id}/resolve` - Resolve alert

### Sensors
- POST `/api/sensor` - Submit sensor reading
- GET `/api/sensor/plant/{plantId}` - Get plant readings
- GET `/api/sensor/plant/{plantId}/latest` - Get latest reading

### Watering
- POST `/api/watering/water/{plantId}` - Water plant

## Future Enhancements

- [ ] Add Plant page with form
- [ ] Plant Detail page with charts
- [ ] Edit Plant functionality
- [ ] Device registration flow
- [ ] Push notifications
- [ ] Photo upload for plant tracking
- [ ] Dark mode support
- [ ] Localization (multiple languages)
- [ ] Offline mode with data caching
- [ ] Real-time updates with SignalR

## License

This project is part of the SmartGarden IoT system.

## Support

For issues or questions:
- Check the backend API is running correctly
- Verify network connectivity
- Review logs in Visual Studio Output window
