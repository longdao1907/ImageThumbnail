using AutoMapper;
using ImageAPI.Core.Application.DTOs;
using ImageAPI.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageAPI.Controllers
{
    [ApiController]
    [Route("api/Image")]
    //[Authorize] // Protect all endpoints in this controller
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private ResponseDto _response;
        private IMapper _mapper;
   
        public ImageController(IImageService imageService, IMapper mapper)
        {
            _imageService = imageService;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpPost("upload-request")]
        public async Task<ResponseDto> Post([FromBody] ImageMetadataDto request)
        {
         
            var _obj = await _imageService.AddImageAsync(request, request.UserId ?? string.Empty);
            _response.Result = _mapper.Map<ImageMetadataDto>(_obj);

            return _response;
        }

        [HttpGet("get-images-by-user")]
        public async Task<ResponseDto> GetUserImages()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Unauthorized Access.";
                    return _response;
                }

                var objList =  await _imageService.GetImagesForUserAsync(userId);
                _response.Result = _mapper.Map<IEnumerable<ImageMetadataDto>>(objList); 

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;


        }

        [HttpGet("get-images")]
        public async Task<ResponseDto> GetImages()
        {
            try
            {
                var objList = await _imageService.GetImagesAsync();
                _response.Result = _mapper.Map<IEnumerable<ImageMetadataDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false; 
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut("update-thumbnail-status")]
        public async Task<ResponseDto> Put([FromBody] ImageMetadataDto request)
        {
            try
            {
                await _imageService.UpdateImageAsync(request);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
