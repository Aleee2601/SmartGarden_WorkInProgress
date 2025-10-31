using Microsoft.Extensions.Logging;
using SmartGarden_WorkInProgress.Services;
using SmartGarden_WorkInProgress.ViewModels;
using SmartGarden_WorkInProgress.Views;

namespace SmartGarden_WorkInProgress;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbols");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.AddSingleton<MockPlantDataService>();

        // Register ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PlantDetailViewModel>();

        // Register Pages
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<PlantDetailPage>();

        return builder.Build();
    }
}
