using ImgThumbnailApp.Web.Models;

namespace ImgThumbnailApp.Web.Services.IServices
{
    public interface IImageService
    {
        Task<ResponseDto> AddImageAsync(ImageMetadataDto request, string userId);
        Task<ResponseDto> GetImagesForUserAsync(string userId);
        Task<ResponseDto> GetImagesAsync();
        Task UpdateImageAsync(ImageMetadataDto imageMetadata);

       
    }
}
