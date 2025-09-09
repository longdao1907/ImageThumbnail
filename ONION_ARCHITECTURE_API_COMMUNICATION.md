# Onion Architecture: API Communication Between Backend Services

## Question: Where to implement communication between 2 backend APIs?

**Answer: Infrastructure Layer**

## Why Infrastructure Layer?

In the onion architecture, **Infrastructure Layer** is the correct place to implement communication between backend APIs for the following reasons:

### 1. **Separation of Concerns**
- **Infrastructure Layer**: Handles external system integrations (HTTP clients, databases, file systems, external APIs)
- **Presentation Layer**: Focuses on user interface concerns, controllers, and request/response mapping
- **Application Layer**: Contains business logic and defines interfaces/contracts
- **Domain Layer**: Contains core business entities and domain logic

### 2. **Dependency Direction**
- Application layer defines **interfaces** (contracts) for external services
- Infrastructure layer **implements** these interfaces
- This follows the **Dependency Inversion Principle**

### 3. **Testability**
- Business logic (Application layer) depends on abstractions, not concrete implementations
- Easy to mock external API calls during testing
- Infrastructure implementations can be swapped without affecting business logic

## Current Implementation Example

In this repository, the `ThumbnailGenerator` service communicates with the `ImageAPI` service:

### Interface Definition (Application Layer)
```csharp
// ThumbnailGenerator/Core/Application/Interfaces/IImageApiClient.cs
namespace ThumbnailGenerator.Core.Application.Interfaces
{
    public enum ThumbnailUpdateStatus { Completed, Failed }
    
    public interface IImageApiClient
    {
        Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null);
    }
}
```

### Implementation (Infrastructure Layer)
```csharp
// ThumbnailGenerator/Infrastructure/APIClients/ImageApiClient.cs
namespace ThumbnailGenerator.Infrastructure.APIClients
{
    public class ImageApiClient : IImageApiClient
    {
        private readonly HttpClient _httpClient;

        public ImageApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null)
        {
            var requestBody = new UpdateThumbnailRequest(thumbnailUrl ?? "", status.ToString());
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"api/image/{imageId}/thumbnail-status", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
```

### Dependency Injection (Presentation/Bootstrap)
```csharp
// Program.cs
builder.Services.AddHttpClient<IImageApiClient, ImageApiClient>(client =>
{
    var baseUrl = builder.Configuration["ImageApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
});
```

## Architecture Benefits

1. **Loose Coupling**: Application layer doesn't know about HTTP, JSON, or specific API endpoints
2. **Easy Testing**: Mock `IImageApiClient` for unit tests
3. **Flexibility**: Swap HTTP communication for gRPC, message queues, etc., without changing business logic
4. **Configuration**: External API URLs and settings managed in Infrastructure layer

## Anti-Pattern: DON'T put API clients in Presentation Layer

❌ **Wrong**: Controllers directly making HTTP calls to external APIs
```csharp
// BAD: Controller directly using HttpClient
[ApiController]
public class ThumbnailController : ControllerBase
{
    private readonly HttpClient _httpClient;
    
    public async Task<IActionResult> ProcessThumbnail(Guid imageId)
    {
        // BAD: Business logic mixed with HTTP communication
        var response = await _httpClient.PatchAsync($"api/image/{imageId}/status", content);
        return Ok();
    }
}
```

✅ **Correct**: Controllers use Application services that depend on Infrastructure abstractions
```csharp
// GOOD: Controller uses Application service
[ApiController]
public class ThumbnailController : ControllerBase
{
    private readonly ThumbnailService _thumbnailService;
    
    public async Task<IActionResult> ProcessThumbnail(Guid imageId)
    {
        await _thumbnailService.ProcessThumbnailAsync(imageId);
        return Ok();
    }
}
```

## Summary

- ✅ **Infrastructure Layer**: Implements API communication (HttpClient, gRPC clients, etc.)
- ✅ **Application Layer**: Defines interfaces/contracts for external services  
- ❌ **Presentation Layer**: Should NOT implement API communication directly
- ❌ **Domain Layer**: Should NOT know about external APIs at all

This pattern ensures clean architecture, testability, and maintainability.