using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using ImageAPI.Core.Application.Interfaces;

namespace ImageAPI.Infrastructure.Services
{
    public class GcsStorageService : IStorageService
    {
        private readonly UrlSigner _urlSigner;
        private readonly string _bucketName;

        public GcsStorageService(IConfiguration configuration)
        {
            _bucketName = configuration["Gcp:BucketName"] ?? throw new ArgumentNullException("GCP BucketName not configured.");

            //Get Application Default Credentials
            var credentials = GoogleCredential.GetApplicationDefault();

            // This implicitly uses Application Default Credentials when running on Google Cloud.
            // For local development, ensure you have authenticated via 'gcloud auth application-default login'.
            _urlSigner =  UrlSigner.FromCredential(credentials);
        }

        public async Task<string> GenerateUploadUrlAsync(string objectName, string contentType)
        {
            // The client will use this URL with an HTTP PUT request.
            var options = UrlSigner.Options.FromDuration(TimeSpan.FromMinutes(15));
            var request = UrlSigner.RequestTemplate
                .FromBucket(_bucketName).WithObjectName(objectName).WithHttpMethod(HttpMethod.Put)
                .WithContentHeaders(new Dictionary<string, IEnumerable<string>>
                {
                { "Content-Type", new[] { contentType } }
                });

            return await _urlSigner.SignAsync(request, options);
        }
    }
}
