using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartGarden_WorkInProgress.Models;
using SmartGarden_WorkInProgress.Services;

namespace SmartGarden_WorkInProgress.ViewModels;

public class PlantDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly MockPlantDataService _dataService;
    private Plant? _plant;
    private bool _isAutoWateringOn;
    private double _wateringIntensity = 50;

    public Plant? Plant
    {
        get => _plant;
        set
        {
            SetProperty(ref _plant, value);
            LoadMetrics();
            LoadWeeklyStats();
        }
    }

    public bool IsAutoWateringOn
    {
        get => _isAutoWateringOn;
        set => SetProperty(ref _isAutoWateringOn, value);
    }

    public double WateringIntensity
    {
        get => _wateringIntensity;
        set => SetProperty(ref _wateringIntensity, value);
    }

    public ObservableCollection<Metric> Metrics { get; } = new();
    public ObservableCollection<WeeklyStat> WeeklyStats { get; } = new();

    public ICommand BackCommand { get; }

    public PlantDetailViewModel(MockPlantDataService dataService)
    {
        _dataService = dataService;
        BackCommand = new Command(OnBack);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Plant", out var plantObj) && plantObj is Plant plant)
        {
            Plant = plant;
        }
    }

    private void LoadMetrics()
    {
        if (Plant == null) return;

        Metrics.Clear();
        Metrics.Add(new Metric
        {
            Label = "Water tank",
            Value = Plant.WaterTank,
            Unit = "%",
            Icon = "water_drop",
            Caption = "Needs refill"
        });
        Metrics.Add(new Metric
        {
            Label = "Light",
            Value = Plant.Light,
            Unit = "",
            Icon = "wb_sunny",
            Caption = "Optimal"
        });
        Metrics.Add(new Metric
        {
            Label = "Temperature",
            Value = Plant.Temperature,
            Unit = "°C",
            Icon = "thermostat",
            Caption = "Good"
        });
        Metrics.Add(new Metric
        {
            Label = "Soil moisture",
            Value = Plant.SoilMoisture,
            Unit = "%",
            Icon = "spa",
            Caption = "Moderate"
        });
    }

    private void LoadWeeklyStats()
    {
        WeeklyStats.Clear();
        foreach (var stat in _dataService.GetWeeklyStats())
        {
            WeeklyStats.Add(stat);
        }
    }

    private async void OnBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
