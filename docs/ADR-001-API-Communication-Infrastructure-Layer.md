# ADR-001: API Communication in Infrastructure Layer

## Status
Accepted

## Context
We need to establish where to implement communication between backend APIs in our onion architecture. The question was: "In the onion architecture, which layer do we implement communication between 2 backend APIs? Presentation or Infrastructure Layer?"

## Decision
We will implement API communication between backend services in the **Infrastructure Layer**, not the Presentation Layer.

## Rationale

### Why Infrastructure Layer?

1. **Separation of Concerns**
   - Infrastructure Layer: Handles external system integrations (HTTP clients, databases, external APIs)
   - Presentation Layer: Handles UI concerns, controllers, request/response mapping
   - Application Layer: Contains business logic and defines service interfaces
   - Domain Layer: Contains core business entities

2. **Dependency Inversion Principle**
   - Application layer defines interfaces (e.g., `IImageApiClient`)
   - Infrastructure layer implements these interfaces (e.g., `ImageApiClient`)
   - Business logic depends on abstractions, not concrete implementations

3. **Testability**
   - Easy to mock external API dependencies during testing
   - Business logic remains testable without external dependencies
   - Fast, reliable unit tests

4. **Flexibility**
   - Can swap HTTP communication for gRPC, message queues, etc.
   - External API changes don't affect business logic
   - Configuration and external concerns isolated in Infrastructure

### Why NOT Presentation Layer?

1. **Violates Single Responsibility Principle**
   - Controllers should focus on HTTP request/response handling
   - Should not contain API communication logic

2. **Tight Coupling**
   - Business logic becomes coupled to HTTP concerns
   - Difficult to test controllers that make external API calls

3. **Poor Separation of Concerns**
   - Mixes UI concerns with external system integration
   - Makes code harder to maintain and understand

## Implementation

### Interface Definition (Application Layer)
```csharp
// Core/Application/Interfaces/IImageApiClient.cs
public interface IImageApiClient
{
    Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null);
}
```

### Implementation (Infrastructure Layer)
```csharp
// Infrastructure/APIClients/ImageApiClient.cs
public class ImageApiClient : IImageApiClient
{
    private readonly HttpClient _httpClient;
    
    public async Task UpdateThumbnailStatusAsync(Guid imageId, ThumbnailUpdateStatus status, string? thumbnailUrl = null)
    {
        // HTTP communication implementation
    }
}
```

### Dependency Injection (Presentation/Bootstrap)
```csharp
// Program.cs
builder.Services.AddHttpClient<IImageApiClient, ImageApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ImageApi:BaseUrl"]);
});
```

## Consequences

### Positive
- ✅ Clean separation of concerns
- ✅ Easy to unit test business logic
- ✅ Flexible implementation swapping
- ✅ Follows onion architecture principles
- ✅ External dependencies properly isolated

### Negative
- ❌ Slightly more complex setup (interfaces + implementations)
- ❌ Need to understand dependency injection patterns

## Examples in Codebase
- `ThumbnailGenerator.Core.Application.Interfaces.IImageApiClient` (interface)
- `ThumbnailGenerator.Infrastructure.APIClients.ImageApiClient` (implementation)
- `ThumbnailGenerator.Core.Application.Interfaces.INotificationApiClient` (example interface)
- `ThumbnailGenerator.Infrastructure.APIClients.NotificationApiClient` (example implementation)

## References
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Onion Architecture by Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)
- [Dependency Inversion Principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle)