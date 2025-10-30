using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGarden.App.Helpers;
using SmartGarden.App.Services;
using SmartGarden.Core.Models;
using System.Collections.ObjectModel;

namespace SmartGarden.App.ViewModels
{
    public partial class PlantsViewModel : BaseViewModel
    {
        private readonly IPlantService _plantService;
        private readonly ISensorService _sensorService;
        private readonly IWateringService _wateringService;

        [ObservableProperty]
        private ObservableCollection<Plant> plants = new();

        [ObservableProperty]
        private Plant? selectedPlant;

        public PlantsViewModel(IPlantService plantService, ISensorService sensorService, IWateringService wateringService)
        {
            _plantService = plantService;
            _sensorService = sensorService;
            _wateringService = wateringService;
            Title = "My Plants";
        }

        public override async Task InitializeAsync()
        {
            await LoadPlantsAsync();
        }

        [RelayCommand]
        private async Task LoadPlantsAsync()
        {
            await ExecuteAsync(async () =>
            {
                var plantsList = await _plantService.GetAllPlantsAsync();
                Plants = new ObservableCollection<Plant>(plantsList);
            });
        }

        [RelayCommand]
        private async Task PlantSelectedAsync(Plant plant)
        {
            if (plant == null)
                return;

            await Shell.Current.GoToAsync($"PlantDetailPage?plantId={plant.PlantId}");
        }

        [RelayCommand]
        private async Task AddPlantAsync()
        {
            await Shell.Current.GoToAsync("AddPlantPage");
        }

        [RelayCommand]
        private async Task WaterPlantAsync(Plant plant)
        {
            if (plant == null)
                return;

            await ExecuteAsync(async () =>
            {
                var success = await _wateringService.WaterPlantAsync(plant.PlantId);
                if (success)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Success",
                        $"{plant.Nickname ?? "Plant"} watered successfully!", "OK");
                }
            }, "Failed to water plant");
        }

        [RelayCommand]
        private async Task RefreshPlantsAsync()
        {
            await LoadPlantsAsync();
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await SecureStorageHelper.ClearAllAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
