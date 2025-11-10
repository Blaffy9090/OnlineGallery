using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Gallery
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<ImageItem> Images { get; set; } = new List<ImageItem>();

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AuthorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CollectionId { get; set; }  // <- Added for collection filter

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> AuthorSelect { get; set; } = new();

        public async Task OnGetAsync()
        {
            var queryable = _db.Images
                               .Include(i => i.Author)
                               .Include(i => i.Collection)
                               .AsQueryable();

            // Filter by search query
            if (!string.IsNullOrWhiteSpace(Query))
            {
                queryable = queryable.Where(i =>
                    i.Title.Contains(Query) ||
                    (i.Author != null && i.Author.Name.Contains(Query)));
            }

            // Filter by author
            if (AuthorId.HasValue)
            {
                queryable = queryable.Where(i => i.AuthorId == AuthorId.Value);
            }

            // Filter by collection
            if (CollectionId.HasValue)
            {
                queryable = queryable.Where(i => i.CollectionId == CollectionId.Value);
            }

            // Sorting
            Images = Sort switch
            {
                "date_asc" => await queryable.OrderBy(i => i.PaintingDate).ToListAsync(),
                "date_desc" => await queryable.OrderByDescending(i => i.PaintingDate).ToListAsync(),
                _ => await queryable.OrderBy(i => i.Title).ToListAsync(),
            };

            // Prepare author select list
            AuthorSelect = await _db.Authors
                                    .OrderBy(a => a.Name)
                                    .Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                                    {
                                        Value = a.Id.ToString(),
                                        Text = a.Name
                                    })
                                    .ToListAsync();
        }
    }
}