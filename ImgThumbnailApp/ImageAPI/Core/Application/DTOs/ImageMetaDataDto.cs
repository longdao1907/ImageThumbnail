namespace ImageAPI.Core.Application.DTOs
{
    public class ImageMetadataDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
    }
}
