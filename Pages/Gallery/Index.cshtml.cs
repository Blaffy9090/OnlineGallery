using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineGallery.Pages.Gallery
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IList<ImageItem> Images { get; set; } = new List<ImageItem>();

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AuthorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CollectionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Tag { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool OnlyLiked { get; set; } = false;

        public List<SelectListItem> AuthorSelect { get; set; } = new();
        public List<SelectListItem> TagSelect { get; set; } = new();

        public async Task OnGetAsync()
        {
            var queryable = _db.Images
                               .Include(i => i.Author)
                               .Include(i => i.Collection)
                               .Include(i => i.Tags)
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

            // Filter by tag
            if (!string.IsNullOrWhiteSpace(Tag))
            {
                queryable = queryable.Where(i => i.Tags.Any(t => t.Name == Tag));
            }

            // Filter only liked by user
            if (OnlyLiked && User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                var likedIds = await _db.Likes
                                        .Where(l => l.UserId == user!.Id)
                                        .Select(l => l.ImageId)
                                        .ToListAsync();
                queryable = queryable.Where(i => likedIds.Contains(i.Id));
            }

            // Sorting
            Images = Sort switch
            {
                "date_asc" => await queryable.OrderBy(i => i.PaintingDate).ToListAsync(),
                "date_desc" => await queryable.OrderByDescending(i => i.PaintingDate).ToListAsync(),
                _ => await queryable.OrderBy(i => i.Title).ToListAsync(),
            };

            // Author select list
            AuthorSelect = await _db.Authors
                                    .OrderBy(a => a.Name)
                                    .Select(a => new SelectListItem
                                    {
                                        Value = a.Id.ToString(),
                                        Text = a.Name
                                    })
                                    .ToListAsync();

            // Tag select list
            var tagNames = await _db.Tags
                        .Select(t => t.Name)
                        .Distinct()
                        .OrderBy(n => n)
                        .ToListAsync();

            TagSelect = tagNames
                        .Select(n => new SelectListItem { Value = n, Text = n })
                        .ToList();

        }
    }
}
