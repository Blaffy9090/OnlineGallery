using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Images
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<ImageItemView> Images { get; set; } = new List<ImageItemView>();
        public string CurrentSort { get; set; } = "";
        public string CurrentFilter { get; set; } = "";
        public string CurrentTag { get; set; } = "";
        public int? MinLikes { get; set; }

        public class ImageItemView
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string? AuthorName { get; set; }
            public string? CollectionTitle { get; set; }
            public DateTime? PaintingDate { get; set; }
            public int LikeCount { get; set; }
            public List<ImageTag> Tags { get; set; } = new();
        }

        public async Task OnGetAsync(string? searchString, string? sortOrder, string? tag, int? minLikes)
        {
            CurrentSort = sortOrder ?? "title";
            CurrentFilter = searchString ?? "";
            CurrentTag = tag ?? "";
            MinLikes = minLikes;

            // Base query
            var query = _db.Images
                .Include(i => i.Author)
                .Include(i => i.Collection)
                .Include(i => i.Tags)
                .AsQueryable();

            // Search by title/author
            if (!string.IsNullOrEmpty(CurrentFilter))
            {
                query = query.Where(i =>
                    i.Title.Contains(CurrentFilter) ||
                    (i.Author != null && i.Author.Name.Contains(CurrentFilter)));
            }

            // Filter by tag
            if (!string.IsNullOrEmpty(CurrentTag))
            {
                query = query.Where(i => i.Tags.Any(t => t.Name == CurrentTag));
            }

            // Project to view model with LikeCount via LINQ
            var imagesQuery = query
                .Select(i => new ImageItemView
                {
                    Id = i.Id,
                    Title = i.Title,
                    AuthorName = i.Author != null ? i.Author.Name : null,
                    CollectionTitle = i.Collection != null ? i.Collection.Title : null,
                    PaintingDate = i.PaintingDate,
                    Tags = i.Tags.ToList(),
                    LikeCount = _db.Likes.Count(l => l.ImageId == i.Id)
                });

            // Filter by minimum likes
            if (MinLikes.HasValue && MinLikes.Value > 0)
            {
                imagesQuery = imagesQuery.Where(i => i.LikeCount >= MinLikes.Value);
            }

            Images = sortOrder switch
            {
                "title_desc" => await imagesQuery.OrderByDescending(i => i.Title).ToListAsync(),
                "author" => await imagesQuery.OrderBy(i => i.AuthorName).ToListAsync(),
                "author_desc" => await imagesQuery.OrderByDescending(i => i.AuthorName).ToListAsync(),
                "date" => await imagesQuery.OrderBy(i => i.PaintingDate).ToListAsync(),
                "date_desc" => await imagesQuery.OrderByDescending(i => i.PaintingDate).ToListAsync(),
                "likes" => await imagesQuery.OrderByDescending(i => i.LikeCount).ToListAsync(),
                _ => await imagesQuery.OrderBy(i => i.Title).ToListAsync(),
            };
        }
    }
}
