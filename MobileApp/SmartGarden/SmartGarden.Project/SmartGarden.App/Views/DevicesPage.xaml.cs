using SmartGarden.App.ViewModels;

namespace SmartGarden.App.Views
{
    public partial class DevicesPage : ContentPage
    {
        private readonly DevicesViewModel _viewModel;

        public DevicesPage(DevicesViewModel viewModel)
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
