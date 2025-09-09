using System.ComponentModel.DataAnnotations;

namespace ImgThumbnailApp.Web.Models
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]

        public string Password { get; set; } = string.Empty;

        [Required]
        public string Name  { get; set; } = string.Empty;
    }
}
