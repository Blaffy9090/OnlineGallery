using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Collections
{
    [Authorize(Roles = "Moderator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public Collection Collection { get; set; } = null!;

        [BindProperty]
        public List<int> SelectedImageIds { get; set; } = new();

        public List<ImageItem> AllImages { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Collection = await _db.Collections.Include(c => c.Images).FirstOrDefaultAsync(c => c.Id == id);
            if (Collection == null) return NotFound();

            AllImages = await _db.Images.Include(i => i.Author).ToListAsync();
            SelectedImageIds = Collection.Images.Select(i => i.Id).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(Collection.Id);
                return Page();
            }

            var dbColl = await _db.Collections.Include(c => c.Images).FirstOrDefaultAsync(c => c.Id == Collection.Id);
            if (dbColl == null) return NotFound();

            dbColl.Title = Collection.Title;
            dbColl.Description = Collection.Description;

            // reset old images
            var oldImgs = await _db.Images.Where(i => i.CollectionId == dbColl.Id).ToListAsync();
            foreach (var img in oldImgs)
                img.CollectionId = null;

            // assign new ones
            if (SelectedImageIds.Any())
            {
                var imgs = await _db.Images.Where(i => SelectedImageIds.Contains(i.Id)).ToListAsync();
                foreach (var img in imgs)
                    img.CollectionId = dbColl.Id;
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
