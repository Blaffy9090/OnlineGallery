using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Images
{
    [Authorize(Roles = "Moderator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) => _db = db;

        [BindProperty] public ImageItem ImageItem { get; set; } = new();
        public SelectList Authors { get; set; } = null!;
        public SelectList Collections { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ImageItem = await _db.Images.FindAsync(id) ?? new ImageItem();
            Authors = new SelectList(await _db.Authors.ToListAsync(), "Id", "Name");
            Collections = new SelectList(await _db.Collections.ToListAsync(), "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            _db.Attach(ImageItem).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
