namespace ThumbnailGenerator.Core.Application.Interfaces
{
    public interface IKmsService
    {
        Task<byte[]> EncryptAsync(byte[] plaintext);
        Task<byte[]> DecryptAsync(byte[] ciphertext);
    }
}