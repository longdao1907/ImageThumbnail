using System.Text;
using System.Text.Json;
using ThumbnailGenerator.Core.Application.Interfaces;

namespace ThumbnailGenerator.Infrastructure.APIClients
{
    /// <summary>
    /// Example Infrastructure layer implementation for communicating with a NotificationAPI service.
    /// This demonstrates the proper placement of API communication logic in onion architecture.
    /// 
    /// Architecture principles demonstrated:
    /// - Infrastructure layer implements interfaces from Application layer
    /// - External communication details (HTTP, JSON, endpoints) are isolated here
    /// - Business logic remains unaware of communication mechanisms
    /// - Implementation can be easily substituted (HTTP → gRPC, REST → GraphQL, etc.)
    /// </summary>
    public class NotificationApiClient : INotificationApiClient
    {
        private readonly HttpClient _httpClient;

        // Private DTO for external API communication
        // Infrastructure layer can define its own data structures for external services
        private record NotificationRequest(string UserId, Guid ImageId, string Status, DateTime Timestamp);

        public NotificationApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Example implementation of backend-to-backend communication.
        /// This shows how Infrastructure layer handles external API calls.
        /// </summary>
        public async Task SendThumbnailNotificationAsync(string userId, Guid imageId, bool isSuccess)
        {
            var status = isSuccess ? "completed" : "failed";
            var requestBody = new NotificationRequest(userId, imageId, status, DateTime.UtcNow);
            
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody), 
                Encoding.UTF8, 
                "application/json");

            // Example endpoint for notification service
            var response = await _httpClient.PostAsync("api/notifications/thumbnail", content);
            
            // Infrastructure layer handles HTTP-specific concerns
            response.EnsureSuccessStatusCode();
        }
    }
}