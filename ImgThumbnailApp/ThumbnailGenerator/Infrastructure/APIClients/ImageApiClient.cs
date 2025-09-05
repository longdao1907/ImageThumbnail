using System.Text;
using System.Text.Json;
using ThumbnailGenerator.Core.Application.Interfaces;

namespace ThumbnailGenerator.Infrastructure.APIClients
{
    public class ImageApiClient : IImageApiClient
    {
        private readonly HttpClient _httpClient;

        // This record is a private DTO for the PATCH request
        private record UpdateThumbnailRequest(string ThumbnailUrl, string Status);

        public ImageApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null)
        {
            var requestBody = new UpdateThumbnailRequest(thumbnailUrl ?? "", status.ToString());
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // Assumes an endpoint like: PATCH /api/images/{imageId}/thumbnail-status
            var response = await _httpClient.PatchAsync($"api/images/{imageId}/thumbnail-status", content);

            response.EnsureSuccessStatusCode();
        }
    }
}
