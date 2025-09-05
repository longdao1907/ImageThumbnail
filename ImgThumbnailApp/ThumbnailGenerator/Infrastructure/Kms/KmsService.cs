using Google.Cloud.Kms.V1;
using Google.Protobuf;
using ThumbnailGenerator.Core.Application.Interfaces;

namespace ThumbnailGenerator.Infrastructure.Kms
{
    public class KmsService : IKmsService
    {
        private readonly KeyManagementServiceClient _kmsClient;
        private readonly CryptoKeyName _keyName;

        public KmsService(IConfiguration configuration)
        {
            _kmsClient = KeyManagementServiceClient.Create();
            var keyPath = configuration["Gcp:KmsKeyPath"] ?? throw new ArgumentNullException("Gcp:KmsKeyPath not configured.");
            _keyName = CryptoKeyName.Parse(keyPath);
        }

        public async Task<byte[]> DecryptAsync(byte[] ciphertext)
        {
            var response = await _kmsClient.DecryptAsync(_keyName, ByteString.CopyFrom(ciphertext));
            return response.Plaintext.ToByteArray();
        }

        public async Task<byte[]> EncryptAsync(byte[] plaintext)
        {
            var response = await _kmsClient.EncryptAsync(_keyName, ByteString.CopyFrom(plaintext));
            return response.Ciphertext.ToByteArray();
        }
    }
}

