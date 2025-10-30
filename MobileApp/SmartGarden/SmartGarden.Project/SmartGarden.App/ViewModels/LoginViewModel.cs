using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGarden.App.Services;

namespace SmartGarden.App.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Login";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter email and password";
                return;
            }

            await ExecuteAsync(async () =>
            {
                var result = await _authService.LoginAsync(Email, Password);
                if (result != null)
                {
                    // Navigate to main page
                    await Shell.Current.GoToAsync("///MainPage");
                }
                else
                {
                    ErrorMessage = "Login failed. Please check your credentials.";
                }
            }, "Login failed");
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
