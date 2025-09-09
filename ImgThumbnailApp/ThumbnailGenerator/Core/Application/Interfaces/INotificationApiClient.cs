namespace ThumbnailGenerator.Core.Application.Interfaces
{
    /// <summary>
    /// Example interface for communicating with a hypothetical NotificationAPI service.
    /// This demonstrates how to define contracts for API-to-API communication in the Application layer.
    /// The actual implementation should be placed in the Infrastructure layer.
    /// </summary>
    public interface INotificationApiClient
    {
        /// <summary>
        /// Sends a notification about thumbnail processing completion to users.
        /// This represents another example of backend-to-backend API communication.
        /// </summary>
        /// <param name="userId">The user to notify</param>
        /// <param name="imageId">The image that was processed</param>
        /// <param name="isSuccess">Whether thumbnail generation was successful</param>
        Task SendThumbnailNotificationAsync(string userId, Guid imageId, bool isSuccess);
    }
}