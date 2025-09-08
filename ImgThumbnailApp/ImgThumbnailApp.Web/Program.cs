using ImgThumbnailApp.Web.Services;
using ImgThumbnailApp.Web.Utilities;
using ImgThumbnailApp.Web.Services.IServices;
using System.Threading.Tasks.Dataflow;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IImageService, ImageService>();  

SD.ImageAPIBase = builder.Configuration["ServiceUrls:ImageAPI"];

builder.Services.AddScoped<IBaseService, BaseService>();    
builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Image}/{action=ImageIndex}/{id?}");

app.Run();
