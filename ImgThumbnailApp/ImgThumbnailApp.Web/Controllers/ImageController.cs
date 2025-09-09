using ImgThumbnailApp.Web.Models;
using ImgThumbnailApp.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ImgThumbnailApp.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;   

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }   
        public async Task<IActionResult> ImageIndex()
        {
            List<ImageMetadataDto>? list = new();

            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto response = await _imageService.GetImagesForUserAsync(userId);

            if (response != null && response.IsSuccess)
            {
                list = JsonSerializer.Deserialize<List<ImageMetadataDto>>(Convert.ToString(response.Result) ?? string.Empty, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select an image file.");
                return View();
            }

            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            // Basic metadata (adjust as needed)
            var metadata = new ImageMetadataDto
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(file.FileName),
                FileSize = file.Length,
                UploadDate = DateTime.UtcNow.ToUniversalTime(),
                ContentType = file.ContentType,
                Status = "Pending",
                UserId = userId,
                GcsObjectName = Guid.NewGuid().ToString(),
                OriginalImageFile = file
            };
            

            var response = await _imageService.AddImageAsync(metadata, metadata.UserId ?? string.Empty);
            if (response == null || !response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Upload failed.");
                TempData["error"] = response.Message;
                return View();
            }

            TempData["success"] = "Successfully Uploaded";
            return RedirectToAction(nameof(ImageIndex));
        }
    }
}
