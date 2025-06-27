using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using SmartGarden.API.Data;
using SmartGarden.API.Models;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
    private readonly SmartGardenContext _context;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(SmartGardenContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<User> Register(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new Exception("Email already registered.");

        var user = new User { Email = email };
        user.PasswordHash = _hasher.HashPassword(user, password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new Exception("Invalid credentials.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result != PasswordVerificationResult.Success)
            throw new Exception("Invalid credentials.");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
