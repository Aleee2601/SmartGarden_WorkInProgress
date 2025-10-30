using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartGarden.App.Services;

namespace SmartGarden.App.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Register";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please fill in all fields";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            await ExecuteAsync(async () =>
            {
                var result = await _authService.RegisterAsync(Email, Password, Name);
                if (result != null)
                {
                    await Shell.Current.GoToAsync("///MainPage");
                }
                else
                {
                    ErrorMessage = "Registration failed. Email may already be in use.";
                }
            }, "Registration failed");
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
