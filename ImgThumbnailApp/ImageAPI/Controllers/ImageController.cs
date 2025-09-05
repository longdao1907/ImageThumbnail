using ImageAPI.Core.Application.DTOs;
using ImageAPI.Core.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageAPI.Controllers
{
    [ApiController]
    [Route("api/Image")]
    [Authorize] // Protect all endpoints in this controller
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload-request")]
        public async Task<IActionResult> GetUploadUrl([FromBody] UploadRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var (uploadUrl, imageId) = await _imageService.CreateUploadUrlAsync(request, userId);

            return Ok(new { uploadUrl, imageId });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserImages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var images = await _imageService.GetImagesForUserAsync(userId);
            return Ok(images);
        }
    }
}
