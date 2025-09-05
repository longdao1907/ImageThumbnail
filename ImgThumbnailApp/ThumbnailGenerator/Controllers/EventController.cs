using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ThumbnailGenerator.Core.Application.Services;
using ThumbnailGenerator.Core.Domain.Models;

namespace ThumbnailGenerator.Controllers
{
    public class EventController : ControllerBase
    {
        private readonly ThumbnailService _thumbnailService;
        private readonly ILogger<EventController> _logger;

        public EventController(ThumbnailService thumbnailService, ILogger<EventController> logger)
        {
            _thumbnailService = thumbnailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CloudEvent cloudEvent)
        {
            // Eventarc sends events with a specific type for GCS uploads
            if (cloudEvent.Type != "google.cloud.storage.object.v1.finalized")
            {
                _logger.LogWarning("Received event with unexpected type: {EventType}", cloudEvent.Type);
                return Ok(); // Acknowledge the event but do nothing
            }

            // The 'Data' property of the CloudEvent is the payload
            if (cloudEvent.Data is null)
            {
                _logger.LogError("Received CloudEvent with null data.");
                return BadRequest("CloudEvent data is required.");
            }

            // Deserialize the data into our strongly-typed model
            var storageObjectData = JsonSerializer.Deserialize<StorageObjectData>(cloudEvent.Data.ToString()!);
            if (storageObjectData is null)
            {
                _logger.LogError("Failed to deserialize StorageObjectData.");
                return BadRequest("Invalid StorageObjectData format.");
            }

            // Ignore events from the destination (thumbnail) bucket to prevent infinite loops
            var destinationBucket = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Gcp:DestinationBucketName"];
            if (storageObjectData.Bucket == destinationBucket)
            {
                _logger.LogInformation("Ignoring event from destination bucket to prevent loop.");
                return Ok();
            }

            await _thumbnailService.ProcessImageAsync(storageObjectData);

            // Always return a 2xx status to Eventarc to prevent retries
            return Ok();
        }
    }
}
