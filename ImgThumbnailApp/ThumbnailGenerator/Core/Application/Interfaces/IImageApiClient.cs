namespace ThumbnailGenerator.Core.Application.Interfaces
{
    public enum ThumbnailUpdateStatus { Completed, Failed }
    
    /// <summary>
    /// Interface for communicating with the ImageAPI service.
    /// This interface is defined in the Application layer (Core) to follow the Dependency Inversion Principle.
    /// The actual implementation belongs in the Infrastructure layer.
    /// </summary>
    public interface IImageApiClient
    {
        /// <summary>
        /// Updates the thumbnail processing status for an image in the ImageAPI service.
        /// This represents communication between two backend services.
        /// </summary>
        /// <param name="imageId">The unique identifier of the image</param>
        /// <param name="status">The status of thumbnail processing</param>
        /// <param name="thumbnailUrl">Optional URL of the generated thumbnail</param>
        Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null);
    }
}
