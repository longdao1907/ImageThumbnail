using ImgThumbnailApp.Web.Models;
using ImgThumbnailApp.Web.Services.IServices;
using ImgThumbnailApp.Web.Utilities;

namespace ImgThumbnailApp.Web.Services
{
    public class ImageService : IImageService
    {
        private readonly IBaseService _baseService;
        public ImageService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> AddImageAsync(ImageMetadataDto imageMetadataDto, string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = Utilities.SD.ApiType.POST,
                Data = imageMetadataDto,
                Url = Utilities.SD.ImageAPIBase + "/api/Image/upload-request",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto> GetImagesAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = Utilities.SD.ApiType.GET,
                Url = Utilities.SD.ImageAPIBase + "/api/Image/get-images"

            });
        }

        public async Task<ResponseDto> GetImagesForUserAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = Utilities.SD.ApiType.GET,
                Url = Utilities.SD.ImageAPIBase + "/api/Image/get-images-by-user/" + userId
            });
        }

        public async Task UpdateImageAsync(ImageMetadataDto imageMetadataDto)
        {
            await _baseService.SendAsync(new RequestDto()
            {
                ApiType = Utilities.SD.ApiType.PUT,
                Data = imageMetadataDto,
                Url = Utilities.SD.ImageAPIBase + "/api/Image/update-image"
            });
        }
    }

}
