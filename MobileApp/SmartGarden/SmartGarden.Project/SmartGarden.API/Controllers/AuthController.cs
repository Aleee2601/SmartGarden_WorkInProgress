using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Login with existing user credentials
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Check if a user exists by email
        /// </summary>
        [HttpGet("exists")]
        public async Task<IActionResult> UserExists([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var exists = await _authService.UserExistsAsync(email);
            return Ok(new { exists });
        }
    }
}
