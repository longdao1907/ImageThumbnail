using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using ThumbnailGenerator.Core.Application.Interfaces;

namespace ThumbnailGenerator.Infrastructure.Services
{
    public class GcsStorageService: IStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _sourceBucketName;
        private readonly string _destinationBucketName;

        public GcsStorageService(IConfiguration configuration)
        {
            _storageClient = StorageClient.Create();
            _sourceBucketName = configuration["Gcp:SourceBucketName"] ?? throw new ArgumentNullException("Gcp:SourceBucketName");
            _destinationBucketName = configuration["Gcp:DestinationBucketName"] ?? throw new ArgumentNullException("Gcp:DestinationBucketName");
        }

        public async Task DownloadFileAsync(string objectName, Stream destination)
        {
            await _storageClient.DownloadObjectAsync(_sourceBucketName, objectName, destination);
        }

        public async Task<string> UploadFileAsync(string objectName, Stream source, string contentType)
        {
            var uploadedObject = await _storageClient.UploadObjectAsync(
                _destinationBucketName,
                objectName,
                contentType,
                source,
                new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
            );
            return uploadedObject.MediaLink;
        }
    }
    
}

