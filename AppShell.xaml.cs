using SmartGarden_WorkInProgress.Views;

namespace SmartGarden_WorkInProgress;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes
        Routing.RegisterRoute(nameof(PlantDetailPage), typeof(PlantDetailPage));
    }
}
