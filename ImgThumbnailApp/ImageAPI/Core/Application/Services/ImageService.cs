
using ImageAPI.Core.Application.DTOs;
using ImageAPI.Core.Application.Interfaces;
using ImageAPI.Core.Domain.Entities;
using ImageAPI.Core.Domain.Enum;

namespace ImageAPI.Core.Application.Services;

public class ImageService
{
    private readonly IImageMetadataRepository _metadataRepository;
    private readonly IStorageService _storageService;

    public ImageService(IImageMetadataRepository metadataRepository, IStorageService storageService)
    {
        _metadataRepository = metadataRepository;
        _storageService = storageService;
    }

    public async Task<(string uploadUrl, Guid imageId)> CreateUploadUrlAsync(UploadRequestDto request, string userId)
    {
        var objectName = $"{userId}/{Guid.NewGuid()}-{request.FileName}";

        var metadata = new ImageMetadata
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName,
            FileSize = request.FileSize,
            ContentType = request.ContentType,
            UploadDate = DateTime.UtcNow,
            Status = ThumbnailStatus.Pending,
            UserId = userId,
            GcsObjectName = objectName
        };

        await _metadataRepository.AddAsync(metadata);

        var uploadUrl = await _storageService.GenerateUploadUrlAsync(objectName, request.ContentType);

        return (uploadUrl, metadata.Id);
    }

    public async Task<IEnumerable<ImageMetadataDto>> GetImagesForUserAsync(string userId)
    {
        var images = await _metadataRepository.GetByUserIdAsync(userId);
        return images.Select(img => new ImageMetadataDto
        {
            Id = img.Id,
            FileName = img.FileName,
            FileSize = img.FileSize,
            UploadDate = img.UploadDate,
            Status = img.Status.ToString(),
            ThumbnailUrl = img.ThumbnailUrl // This will be populated later by the thumbnail service
        }).OrderByDescending(i => i.UploadDate);
    }
}