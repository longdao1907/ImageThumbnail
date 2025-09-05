using AuthAPI.Core.Application.DTOs;
using AuthAPI.Core.Application.Interfaces;
using AuthAPI.Core.Domain.Entities;

namespace AuthAPI.Core.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(LoginRequestDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser == null)
            {
                return null; // User not found
            }

            var isPasswordValid = _passwordService.VerifyPassword(request.Password, existingUser.PasswordHash);
            if (!isPasswordValid)
            {
                return null; // Invalid password
            }

            return _tokenService.CreateToken(existingUser);
        }

        public async Task<UserDto?> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return null; // User already exists
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new UserDto { Id = user.Id, Email = user.Email };
        }
    }
}



