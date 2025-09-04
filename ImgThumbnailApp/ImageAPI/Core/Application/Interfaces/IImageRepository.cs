using ImageAPI.Core.Domain.Entities;

namespace ImageAPI.Core.Application.Interfaces
{
    public interface IImageMetadataRepository
    {
        Task AddAsync(ImageMetadata metadata);
        Task<ImageMetadata?> GetByIdAsync(Guid id);
        Task<IEnumerable<ImageMetadata>> GetByUserIdAsync(string userId);
        Task UpdateAsync(ImageMetadata metadata);
    }
}
