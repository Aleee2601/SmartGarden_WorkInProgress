using SmartGarden.App.Helpers;

namespace SmartGarden.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell()) { Title = "SmartGarden" };
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Check if user is logged in
            var token = await SecureStorageHelper.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                // Not logged in, go to login page
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                // Logged in, go to main page
                await Shell.Current.GoToAsync("///MainPage");
            }
        }
    }
}