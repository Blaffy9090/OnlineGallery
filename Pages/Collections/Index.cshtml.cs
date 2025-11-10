using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Collections
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<Collection> Collections { get; set; } = new List<Collection>();

        public async Task OnGetAsync()
        {
            // Load collections including images
            Collections = await _db.Collections
                                   .Include(c => c.Images)
                                   .OrderBy(c => c.Title)
                                   .ToListAsync();
        }
    }
}
