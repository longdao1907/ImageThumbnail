using System.ComponentModel.DataAnnotations;

namespace ImageAPI.Core.Application.DTOs
{ 
    public class UploadRequestDto
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Range(1, 50 * 1024 * 1024)] // Example: 50 MB limit
        public long FileSize { get; set; }
    }
}
