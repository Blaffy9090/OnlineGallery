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
        public List<string> Tags { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ImageItem = await _db.Images.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id) ?? new ImageItem();
            Authors = new SelectList(await _db.Authors.ToListAsync(), "Id", "Name");
            Collections = new SelectList(await _db.Collections.ToListAsync(), "Id", "Title");
            Tags = ImageItem.Tags.Select(t => t.Name).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string TagNames)
        {
            if (!ModelState.IsValid) return Page();

            _db.Attach(ImageItem).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            // Update tags
            var oldTags = await _db.Tags.Where(t => t.ImageId == ImageItem.Id).ToListAsync();
            _db.Tags.RemoveRange(oldTags);

            if (!string.IsNullOrWhiteSpace(TagNames))
            {
                foreach (var t in TagNames.Split(',').Select(s => s.Trim()).Where(s => s != ""))
                {
                    _db.Tags.Add(new ImageTag { Name = t, ImageId = ImageItem.Id });
                }
            }
            await _db.SaveChangesAsync();

            return RedirectToPage("/Images/Details", new { id = ImageItem.Id });
        }
    }
}