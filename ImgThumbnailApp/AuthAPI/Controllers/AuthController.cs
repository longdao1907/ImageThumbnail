using AuthAPI.Core.Application.DTOs;
using AuthAPI.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("Email is already taken.");
            }
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var token = await _authService.LoginAsync(request);
            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(new { Token = token });
        }
    }
}
