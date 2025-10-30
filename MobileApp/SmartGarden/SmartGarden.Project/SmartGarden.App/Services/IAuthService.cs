using SmartGarden.Core.DTOs;

namespace SmartGarden.App.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(string email, string password);
        Task<AuthResponseDto?> RegisterAsync(string email, string password, string name);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
