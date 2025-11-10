using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Authors
{
    [Authorize(Roles = "Moderator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public Author Author { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Author = await _db.Authors.FindAsync(id);
            if (Author == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _db.Attach(Author).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Authors.AnyAsync(a => a.Id == Author.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }
    }
}
