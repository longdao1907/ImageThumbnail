using ImgThumbnailApp.Web.Models;

namespace ImgThumbnailApp.Web.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegisterRequestDto registrationRequestDto);
    }
}
