using SmartGarden.App.ViewModels;

namespace SmartGarden.App.Views
{
    public partial class PlantsPage : ContentPage
    {
        private readonly PlantsViewModel _viewModel;

        public PlantsPage(PlantsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}
