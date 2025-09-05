
using AuthAPI.Core.Application.DTOs;

namespace AuthAPI.Core.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto?> RegisterAsync(RegisterRequestDto request);
        Task<string?> LoginAsync(LoginRequestDto request);

    }
}
