using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly SmartGardenDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(SmartGardenDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // Check if user already exists
            if (await UserExistsAsync(dto.Email))
            {
                return null; // User already exists
            }

            // Hash the password
            var passwordHash = HashPassword(dto.Password);

            // Create new user
            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            // Create default user settings
            var userSetting = new UserSetting
            {
                UserId = user.UserId,
                AutoWateringEnabled = false,
                SoilMoistThreshold = 30.0,
                DataReadIntervalMin = 15
            };

            _context.UserSettings.Add(userSetting);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return null; // User not found
            }

            // Verify password
            if (!VerifyPassword(dto.Password, user.PasswordHash))
            {
                return null; // Invalid password
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "SmartGardenAPI";
            var audience = jwtSettings["Audience"] ?? "SmartGardenClient";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == passwordHash;
        }
    }
}
