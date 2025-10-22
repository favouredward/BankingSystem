using BankingSystem.Domain.DTOs;
using BankingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous] // Allow anyone to access this endpoint
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            var (success, error) = await _authService.RegisterUserAsync(model);

            if (!success)
            {
                return BadRequest(new { Message = "Registration failed.", Errors = error });
            }

            return Ok(new { Message = "User registered successfully." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous] // Allow anyone to access this endpoint
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var (success, token, userId, error) = await _authService.LoginAsync(model);

            if (!success)
            {
                return Unauthorized(new { Message = error });
            }

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = userId,
                Email = model.Email
            });
        }
    }
}