# SmartGarden - Plant Dashboard MAUI App

A modern, pixel-perfect .NET MAUI mobile application for plant monitoring and care management.

## Features

- **Dashboard View**: Browse all your plants with beautiful gradient cards
- **Room Filtering**: Filter plants by room location (Bedroom, Living Room, Kitchen, etc.)
- **Plant Detail View**: Comprehensive metrics including humidity, temperature, soil moisture, and more
- **Automatic Watering Control**: Toggle and configure automated watering schedules
- **Weekly Stats**: Visual bar chart showing weekly plant health metrics
- **Tips & Tricks**: Helpful cards with plant care guidance
- **Menu System**: Access profile, add plants, calibrate sensors, and more

## Architecture

Built using **MVVM (Model-View-ViewModel)** pattern with:
- Clean separation of concerns
- Dependency injection
- Data binding
- Command pattern for user interactions

## Project Structure

```
SmartGarden_WorkInProgress/
├── Models/                      # Data models (Plant, Metric, TipCard, etc.)
├── ViewModels/                  # ViewModels with business logic
├── Views/                       # XAML pages (Dashboard, PlantDetail)
├── Controls/                    # Reusable custom controls
├── Converters/                  # Value converters for data binding
├── Services/                    # Mock data service
├── Resources/
│   ├── Fonts/                   # Material Symbols font
│   ├── Images/                  # Plant and avatar images
│   └── Styles/                  # Color palette and styles
├── App.xaml                     # Application resources
├── AppShell.xaml                # Navigation shell
└── MauiProgram.cs               # Service registration
```

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 (17.8+) or Visual Studio Code with .NET MAUI extension
- Android SDK (for Android development)
- Xcode (for iOS development on macOS)

## Setup Instructions

### 1. Navigate to Project

```bash
cd SmartGarden_WorkInProgress
```

### 2. Download Required Font

The app uses **Material Symbols Outlined** font for icons.

1. Download from: https://github.com/google/material-design-icons/raw/master/variablefont/MaterialSymbolsOutlined%5BFILL%2CGRAD%2Copsz%2Cwght%5D.ttf
2. Rename to `MaterialSymbolsOutlined.ttf`
3. Place in `Resources/Fonts/` folder

### 3. Add Plant Images (Optional)

Place these placeholder images in `Resources/Images/`:
- `plant_ficus.png`
- `plant_monstera.png`
- `avatar_placeholder.png`

*Note: The app will display colored placeholders if images are not provided.*

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Build the Project

```bash
dotnet build
```

### 6. Run on Android

```bash
dotnet build -t:Run -f net8.0-android
```

Or use Visual Studio:
1. Set Android emulator as startup project
2. Press F5 to run

### 7. Run on iOS (macOS only)

```bash
dotnet build -t:Run -f net8.0-ios
```

Or use Visual Studio for Mac:
1. Set iOS simulator as startup project
2. Press F5 to run

## Color Palette

- **Primary Green**: `#1AA44B`
- **Dark Green**: `#0E7A2B`
- **Accent**: `#2CCB73`
- **Surface**: `#FFFFFF`
- **Surface Alt**: `#F3F4F6`
- **Text**: `#0F172A`
- **Subtle**: `#64748B`
- **Danger**: `#E11D48`

## Custom Controls

### Badge
Displays status badges with variants (Success, Neutral, Danger).

### MetricPill
Shows plant metrics with icon, value, unit, and caption.

### TipCardView
Displays helpful tips with optional gradient emphasis.

### WeeklyStatsChart
Bar chart visualization using Grid and BoxView (no 3rd-party dependencies).

### AutoWateringControl
Toggle switch with segmented control and intensity slider.

## Mock Data

The app includes a `MockPlantDataService` with sample data:
- **Ficus lyrata**: 26 weeks old, low water tank (10%)
- **Monstera Deliciosa**: 18 weeks old
- **Snake Plant**: 12 weeks old
- **Pothos**: 8 weeks old
- **Peace Lily**: 22 weeks old
- **Rosemary**: 15 weeks old (outdoor)

## Navigation

- **Dashboard → Plant Detail**: Tap any plant card
- **Plant Detail → Dashboard**: Tap back button
- **Menu**: Tap settings gear icon

## Dark Mode Support

The app includes dark mode color definitions. To enable:
1. Update `App.xaml` to merge dark theme dictionary based on system theme
2. Add `AppThemeColor` resources for dynamic color switching

## Known Limitations

- Images are placeholders (BoxView) until actual plant images are added
- Menu actions show alerts (not fully implemented)
- "Add a new plant" route is registered but page not implemented
- No backend integration (using mock data)

## Troubleshooting

### Build Errors
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`

### Font Not Showing
- Verify `MaterialSymbolsOutlined.ttf` is in `Resources/Fonts/`
- Check font is registered in `MauiProgram.cs`
- Clean bin/obj folders and rebuild

### App Crashes on Startup
- Check Output window for detailed error messages
- Ensure all dependencies are restored
- Verify MAUI workload is installed: `dotnet workload install maui`

## Development Notes

- All XAML follows Microsoft .NET MAUI best practices
- ViewModels implement `INotifyPropertyChanged` via `BaseViewModel`
- Navigation uses Shell routing
- Dependency injection configured in `MauiProgram.cs`
- No code-behind logic (except minimal initialization)
- Responsive design using FlexLayout and Grid

## Future Enhancements

- [ ] Add backend API integration
- [ ] Implement actual plant image loading
- [ ] Create "Add Plant" page
- [ ] Add sensor calibration functionality
- [ ] Implement user profile management
- [ ] Add push notifications for watering reminders
- [ ] Export data functionality
- [ ] Plant care history tracking
- [ ] Social sharing features

## License

This project is created for demonstration purposes.
