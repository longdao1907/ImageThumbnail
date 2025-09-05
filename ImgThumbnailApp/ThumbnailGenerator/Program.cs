using CloudNative.CloudEvents;
using ThumbnailGenerator.Core.Application.Interfaces;
using ThumbnailGenerator.Core.Application.Services;
using ThumbnailGenerator.Infrastructure.APIClients;
using ThumbnailGenerator.Infrastructure.Kms;
using ThumbnailGenerator.Infrastructure.Services;
using CloudNative.CloudEvents.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.

// Register application and infrastructure services for DI
builder.Services.AddSingleton<IStorageService, GcsStorageService>();
builder.Services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
builder.Services.AddSingleton<IKmsService, KmsService>();
builder.Services.AddScoped<ThumbnailService>();

// Configure a typed HttpClient for the ImageApiClient
builder.Services.AddHttpClient<IImageApiClient, ImageApiClient>(client =>
{
    var baseUrl = builder.Configuration["ImageApi:BaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
    {
        throw new InvalidOperationException("ImageApi:BaseUrl is not configured.");
    }
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddControllers();
// Add the CloudEvents middleware to automatically parse incoming requests
builder.Services.AddCloudEvents();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// The CloudEvents middleware needs to be registered before routing
app.UseCloudEvents();

app.MapControllers();

// Map a default endpoint for health checks (required by some cloud providers)
app.MapGet("/", () => "ThumbnailGenerator is healthy.");

app.Run();