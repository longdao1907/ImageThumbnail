# Unit Testing API Communication in Onion Architecture

## Why Testing is Easy with Infrastructure Layer Pattern

When API communication is properly placed in the Infrastructure layer, testing becomes straightforward because:

1. **Business logic depends on interfaces** (from Application layer)
2. **Infrastructure implementations can be easily mocked**
3. **External dependencies are isolated**

## Example: Testing ThumbnailService

Here's how you would test a service that uses API communication:

### Service Under Test (Application Layer)
```csharp
// ThumbnailGenerator/Core/Application/Services/ThumbnailService.cs
public class ThumbnailService
{
    private readonly IImageApiClient _imageApiClient;
    private readonly INotificationApiClient _notificationApiClient;
    
    public ThumbnailService(
        IImageApiClient imageApiClient, 
        INotificationApiClient notificationApiClient)
    {
        _imageApiClient = imageApiClient;
        _notificationApiClient = notificationApiClient;
    }
    
    public async Task ProcessThumbnailAsync(Guid imageId, string userId)
    {
        try
        {
            // Process thumbnail logic here...
            var thumbnailUrl = "https://example.com/thumb.jpg";
            
            // Update status via API communication (Infrastructure layer)
            await _imageApiClient.UpdateThumbnailStatusAsync(
                imageId, 
                ThumbnailUpdateStatus.Completed, 
                thumbnailUrl);
                
            // Send notification via API communication (Infrastructure layer)
            await _notificationApiClient.SendThumbnailNotificationAsync(
                userId, 
                imageId, 
                true);
        }
        catch (Exception)
        {
            // Handle failure
            await _imageApiClient.UpdateThumbnailStatusAsync(
                imageId, 
                ThumbnailUpdateStatus.Failed);
                
            await _notificationApiClient.SendThumbnailNotificationAsync(
                userId, 
                imageId, 
                false);
        }
    }
}
```

### Unit Test Example
```csharp
[Test]
public async Task ProcessThumbnailAsync_Success_UpdatesStatusAndSendsNotification()
{
    // Arrange
    var imageId = Guid.NewGuid();
    var userId = "user123";
    
    var mockImageApiClient = new Mock<IImageApiClient>();
    var mockNotificationApiClient = new Mock<INotificationApiClient>();
    
    var service = new ThumbnailService(
        mockImageApiClient.Object,
        mockNotificationApiClient.Object);
    
    // Act
    await service.ProcessThumbnailAsync(imageId, userId);
    
    // Assert - Verify API calls were made correctly
    mockImageApiClient.Verify(x => x.UpdateThumbnailStatusAsync(
        imageId,
        ThumbnailUpdateStatus.Completed,
        It.IsAny<string>()), Times.Once);
        
    mockNotificationApiClient.Verify(x => x.SendThumbnailNotificationAsync(
        userId,
        imageId,
        true), Times.Once);
}

[Test]
public async Task ProcessThumbnailAsync_Failure_UpdatesStatusAsFailed()
{
    // Arrange
    var imageId = Guid.NewGuid();
    var userId = "user123";
    
    var mockImageApiClient = new Mock<IImageApiClient>();
    var mockNotificationApiClient = new Mock<INotificationApiClient>();
    
    // Setup to throw exception during processing
    mockImageApiClient.Setup(x => x.UpdateThumbnailStatusAsync(
        It.IsAny<Guid>(),
        ThumbnailUpdateStatus.Completed,
        It.IsAny<string>()))
        .ThrowsAsync(new HttpRequestException("API unavailable"));
    
    var service = new ThumbnailService(
        mockImageApiClient.Object,
        mockNotificationApiClient.Object);
    
    // Act
    await service.ProcessThumbnailAsync(imageId, userId);
    
    // Assert - Verify failure handling
    mockImageApiClient.Verify(x => x.UpdateThumbnailStatusAsync(
        imageId,
        ThumbnailUpdateStatus.Failed,
        null), Times.Once);
        
    mockNotificationApiClient.Verify(x => x.SendThumbnailNotificationAsync(
        userId,
        imageId,
        false), Times.Once);
}
```

## Benefits of This Pattern

### ✅ Easy to Test
- Mock interfaces instead of concrete HTTP clients
- Test business logic without external dependencies
- Verify API calls were made with correct parameters

### ✅ Fast Tests
- No actual HTTP calls during unit tests
- Tests run quickly and reliably
- No need for test servers or external services

### ✅ Reliable Tests
- Tests don't fail due to network issues
- No flaky tests from external API dependencies
- Consistent test results

## Integration Testing

For integration tests, you can:

1. **Use Test Containers**: Spin up real services for integration tests
2. **Use WireMock**: Mock external APIs at the HTTP level
3. **Use Test Doubles**: Create in-memory implementations for testing

### Example Integration Test Setup
```csharp
[Test]
public async Task ImageApiClient_UpdateThumbnailStatus_CallsCorrectEndpoint()
{
    // Arrange
    using var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    
    var imageApiClient = new ImageApiClient(client);
    
    // Act & Assert
    await imageApiClient.UpdateThumbnailStatusAsync(
        Guid.NewGuid(), 
        ThumbnailUpdateStatus.Completed, 
        "https://example.com/thumb.jpg");
    
    // Verify actual HTTP call was made (using WireMock or test server)
}
```

## Summary

The Infrastructure layer pattern for API communication enables:
- **Easy unit testing** with mocks
- **Fast, reliable tests** without external dependencies  
- **Clear separation** between business logic and external concerns
- **Flexible implementation** changes without affecting tests