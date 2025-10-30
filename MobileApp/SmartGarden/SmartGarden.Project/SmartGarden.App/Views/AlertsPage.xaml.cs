using SmartGarden.App.ViewModels;

namespace SmartGarden.App.Views
{
    public partial class AlertsPage : ContentPage
    {
        private readonly AlertsViewModel _viewModel;

        public AlertsPage(AlertsViewModel viewModel)
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
