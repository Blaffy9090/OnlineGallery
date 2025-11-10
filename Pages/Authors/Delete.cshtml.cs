using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Authors
{
    [Authorize(Roles = "Moderator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DeleteModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public Author Author { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Author = await _db.Authors.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
            if (Author == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var author = await _db.Authors.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == Author.Id);
            if (author == null)
                return NotFound();

            // optional: detach images if any (prevent FK constraint)
            foreach (var img in author.Images)
                img.AuthorId = null;

            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
