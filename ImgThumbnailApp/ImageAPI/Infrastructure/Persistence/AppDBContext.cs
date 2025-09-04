using ImageAPI.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageAPI.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ImageMetadata> ImageMetadata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties, keys, and relationships here if needed
            modelBuilder.Entity<ImageMetadata>().HasKey(im => im.Id);
            modelBuilder.Entity<ImageMetadata>().HasIndex(im => im.UserId);
        }
    }
}
