using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SmartGarden.App.Converters;
using SmartGarden.App.Services;
using SmartGarden.App.ViewModels;
using SmartGarden.App.Views;

namespace SmartGarden.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Register Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IPlantService, PlantService>();
            builder.Services.AddSingleton<IDeviceService, DeviceService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<ISensorService, SensorService>();
            builder.Services.AddSingleton<IWateringService, WateringService>();

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<PlantsViewModel>();
            builder.Services.AddTransient<DevicesViewModel>();
            builder.Services.AddTransient<AlertsViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<PlantsPage>();
            builder.Services.AddTransient<DevicesPage>();
            builder.Services.AddTransient<AlertsPage>();

            // Register Converters globally
            builder.Services.AddSingleton<StringIsNotNullOrEmptyConverter>();
            builder.Services.AddSingleton<InvertedBoolConverter>();

            return builder.Build();
        }
    }
}
