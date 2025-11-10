using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Collections
{
    [Authorize(Roles = "Moderator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DeleteModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public Collection? Collection { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Collection = await _db.Collections.Include(c => c.Images).FirstOrDefaultAsync(c => c.Id == id);
            if (Collection == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Collection == null) return NotFound();

            var coll = await _db.Collections.Include(c => c.Images).FirstOrDefaultAsync(c => c.Id == Collection.Id);
            if (coll == null) return NotFound();

            // Unlink images before deleting
            foreach (var img in coll.Images)
                img.CollectionId = null;

            _db.Collections.Remove(coll);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
