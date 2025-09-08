
using AutoMapper;
using ImageAPI.Core.Application.DTOs;
using ImageAPI.Core.Application.Interfaces;
using ImageAPI.Core.Domain.Entities;
using ImageAPI.Core.Domain.Enum;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ImageAPI.Core.Application.Services;

public class ImageService : IImageService
{
    private readonly IImageMetadataRepository _metadataRepository;
    private readonly IStorageService _storageService;
    private readonly IMapper _mapper;

    public ImageService(IImageMetadataRepository metadataRepository, IStorageService storageService, IMapper mapper)
    {
        _metadataRepository = metadataRepository;
        _storageService = storageService;
        _mapper = mapper;
    }

    public async Task<ImageMetadataDto> AddImageAsync(ImageMetadataDto request, string userId)
    {
        var objectName = $"{Guid.NewGuid()}-{request.FileName}";
        var metadata = _mapper.Map<ImageMetadata>(request);

        string uploadUrl = string.Empty;

        if (request.OriginalImageFile != null || request.OriginalImageFile.Length > 0)
        {
            var source = request.OriginalImageFile.OpenReadStream();

            metadata.GcsObjectName = await _storageService.UploadFileAsync(objectName, source, request.ContentType);
        }

        

        await _metadataRepository.AddAsync(metadata);

        

        return _mapper.Map<ImageMetadataDto>(metadata);
    }

    public async Task<IEnumerable<ImageMetadataDto>> GetImagesForUserAsync(string userId)
    {
        var images = await _metadataRepository.GetByUserIdAsync(userId);
        return images.Select(img => _mapper.Map<ImageMetadataDto>(img)).OrderByDescending(i => i.UploadDate);
    }

    public async Task<IEnumerable<ImageMetadataDto>> GetImagesAsync()
    {
        var images = await _metadataRepository.GetImages();
        return images.Select(img => _mapper.Map<ImageMetadataDto>(img)).OrderByDescending(i => i.UploadDate);
    }

    public async Task UpdateImageAsync(ImageMetadataDto imageMetadata)
    {
        var metadata = _mapper.Map<ImageMetadata>(imageMetadata);
        if (metadata != null)
        {
            await _metadataRepository.UpdateAsync(metadata);
        }

    }





}