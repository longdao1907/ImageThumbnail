namespace ThumbnailGenerator.Core.Application.Interfaces
{
    public enum ThumbnailUpdateStatus { Completed, Failed }
    public interface IImageApiClient
    {
        Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null);
    }
}
