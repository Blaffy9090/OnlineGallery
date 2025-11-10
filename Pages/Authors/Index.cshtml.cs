using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Authors
{
    [Authorize(Roles = "Moderator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<Author> Authors { get; set; } = new List<Author>();

        public async Task OnGetAsync()
        {
            Authors = await _db.Authors.Include(a => a.Images).OrderBy(a => a.Name).ToListAsync();
        }
    }
}
