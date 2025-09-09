using System.Text;
using System.Text.Json;
using ThumbnailGenerator.Core.Application.Interfaces;

namespace ThumbnailGenerator.Infrastructure.APIClients
{
    /// <summary>
    /// Infrastructure layer implementation for communicating with the ImageAPI service.
    /// This class demonstrates the correct place for API-to-API communication in onion architecture.
    /// 
    /// Key principles:
    /// - Implements interface defined in Application layer (IImageApiClient)
    /// - Handles external HTTP communication details (HttpClient, JSON serialization, etc.)
    /// - Business logic in Application layer depends on the interface, not this implementation
    /// - Can be easily mocked for testing
    /// </summary>
    public class ImageApiClient : IImageApiClient
    {
        private readonly HttpClient _httpClient;

        // This record is a private DTO for the PATCH request
        // Infrastructure layer can define its own DTOs for external API communication
        private record UpdateThumbnailRequest(string ThumbnailUrl, string Status);

        public ImageApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Communicates with another backend service (ImageAPI) to update thumbnail status.
        /// This is the proper way to implement inter-service communication in the Infrastructure layer.
        /// </summary>
        public async Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null)
        {
            var requestBody = new UpdateThumbnailRequest(thumbnailUrl ?? "", status.ToString());
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // Assumes an endpoint like: PATCH /api/images/{imageId}/thumbnail-status
            var response = await _httpClient.PatchAsync($"api/image/{imageId}/thumbnail-status", content);

            response.EnsureSuccessStatusCode();
        }
    }
}
