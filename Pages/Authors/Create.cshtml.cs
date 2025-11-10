using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Authors
{
    [Authorize(Roles = "Moderator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public CreateModel(ApplicationDbContext db) => _db = db;

        [BindProperty] public Author Author { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _db.Authors.Add(Author);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
