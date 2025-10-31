using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartGarden_WorkInProgress.Models;
using SmartGarden_WorkInProgress.Services;
using SmartGarden_WorkInProgress.Views;

namespace SmartGarden_WorkInProgress.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly MockPlantDataService _dataService;
    private List<Plant> _allPlants;
    private bool _isMenuOpen;

    public ObservableCollection<Plant> Plants { get; } = new();
    public ObservableCollection<TipCard> Tips { get; } = new();
    public ObservableCollection<RoomFilter> Rooms { get; } = new();

    public string UserName => "Alexandra";

    public bool IsMenuOpen
    {
        get => _isMenuOpen;
        set => SetProperty(ref _isMenuOpen, value);
    }

    public ICommand SelectRoomCommand { get; }
    public ICommand PlantTappedCommand { get; }
    public ICommand ShowMenuCommand { get; }
    public ICommand MenuItemCommand { get; }

    public DashboardViewModel(MockPlantDataService dataService)
    {
        _dataService = dataService;
        _allPlants = new List<Plant>();

        SelectRoomCommand = new Command<RoomFilter>(OnSelectRoom);
        PlantTappedCommand = new Command<Plant>(OnPlantTapped);
        ShowMenuCommand = new Command(OnShowMenu);
        MenuItemCommand = new Command<string>(OnMenuItemSelected);

        LoadData();
    }

    private void LoadData()
    {
        _allPlants = _dataService.GetPlants();

        foreach (var plant in _allPlants)
            Plants.Add(plant);

        foreach (var tip in _dataService.GetTips())
            Tips.Add(tip);

        foreach (var room in _dataService.GetRoomFilters(_allPlants))
            Rooms.Add(room);
    }

    private void OnSelectRoom(RoomFilter? selectedRoom)
    {
        if (selectedRoom == null) return;

        foreach (var room in Rooms)
            room.IsSelected = room.Name == selectedRoom.Name;

        Plants.Clear();
        var filteredPlants = selectedRoom.Name == "All"
            ? _allPlants
            : _allPlants.Where(p => p.Room == selectedRoom.Name);

        foreach (var plant in filteredPlants)
            Plants.Add(plant);
    }

    private async void OnPlantTapped(Plant? plant)
    {
        if (plant == null) return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Plant", plant }
        };

        await Shell.Current.GoToAsync(nameof(PlantDetailPage), navigationParameter);
    }

    private void OnShowMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    private async void OnMenuItemSelected(string? action)
    {
        IsMenuOpen = false;

        if (string.IsNullOrEmpty(action)) return;

        await Application.Current?.MainPage?.DisplayAlert("Menu Action", $"Selected: {action}", "OK")!;
    }
}
