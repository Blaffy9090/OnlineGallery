using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Images
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DetailsModel(ApplicationDbContext db) => _db = db;

        public ImageItem Image { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var img = await _db.Images
                .Include(i => i.Author)
                .Include(i => i.Collection)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (img == null)
                return NotFound();

            Image = img;
            return Page();
        }
    }
}
