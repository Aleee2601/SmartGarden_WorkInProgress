using SmartGarden.App.Constants;
using SmartGarden.App.Helpers;
using SmartGarden.Core.DTOs;

namespace SmartGarden.App.Services
{
    public class AuthService : BaseApiService, IAuthService
    {
        public AuthService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<AuthResponseDto?> LoginAsync(string email, string password)
        {
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            var response = await PostAsync<LoginDto, AuthResponseDto>(ApiConstants.Login, loginDto);

            if (response != null)
            {
                // Store auth data
                await SecureStorageHelper.SetTokenAsync(response.Token);
                await SecureStorageHelper.SetUserIdAsync(response.UserId);
                await SecureStorageHelper.SetUserEmailAsync(response.Email);
            }

            return response;
        }

        public async Task<AuthResponseDto?> RegisterAsync(string email, string password, string name)
        {
            var registerDto = new RegisterDto
            {
                Email = email,
                Password = password,
                Name = name
            };

            var response = await PostAsync<RegisterDto, AuthResponseDto>(ApiConstants.Register, registerDto);

            if (response != null)
            {
                // Store auth data
                await SecureStorageHelper.SetTokenAsync(response.Token);
                await SecureStorageHelper.SetUserIdAsync(response.UserId);
                await SecureStorageHelper.SetUserEmailAsync(response.Email);
            }

            return response;
        }

        public async Task LogoutAsync()
        {
            await SecureStorageHelper.ClearAllAsync();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorageHelper.GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
