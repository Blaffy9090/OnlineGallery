using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Images
{
    [Authorize(Roles = "Moderator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public DeleteModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public ImageItem ImageItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ImageItem = await _context.Images.FindAsync(id);
            if (ImageItem == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, image.FileName ?? "");
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}