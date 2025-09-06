using ImgThumbnailApp.Web.Models;

namespace ImgThumbnailApp.Web.Services.IServices
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto);

    }
}
