using SmartGarden.Core.DTOs;

namespace SmartGarden.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<bool> UserExistsAsync(string email);
    }
}
