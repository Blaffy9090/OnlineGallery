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

        public IList<ImageItem> Images { get; set; } = new List<ImageItem>();
        public string CurrentSort { get; set; } = "";
        public string CurrentFilter { get; set; } = "";

        public async Task OnGetAsync(string? searchString, string? sortOrder)
        {
            CurrentSort = sortOrder ?? "title";
            CurrentFilter = searchString ?? "";

            var query = _db.Images
                           .Include(i => i.Author)
                           .Include(i => i.Collection)
                           .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(i => i.Title.Contains(searchString) ||
                                         (i.Author != null && i.Author.Name.Contains(searchString)));
            }

            Images = sortOrder switch
            {
                "title_desc" => await query.OrderByDescending(i => i.Title).ToListAsync(),
                "author" => await query.OrderBy(i => i.Author!.Name).ToListAsync(),
                "author_desc" => await query.OrderByDescending(i => i.Author!.Name).ToListAsync(),
                "date" => await query.OrderBy(i => i.PaintingDate).ToListAsync(),
                "date_desc" => await query.OrderByDescending(i => i.PaintingDate).ToListAsync(),
                _ => await query.OrderBy(i => i.Title).ToListAsync(),
            };
        }
    }
}
