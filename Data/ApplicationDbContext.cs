using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Models;

namespace OnlineGallery.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<ImageItem> Images { get; set; } = null!;
        public DbSet<Collection> Collections { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ImageItem>()
                .HasOne(i => i.Author)
                .WithMany(a => a.Images)
                .HasForeignKey(i => i.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ImageItem>()
                .HasOne(i => i.Collection)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.CollectionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
