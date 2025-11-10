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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CreateModel(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public SelectList AuthorSelect { get; set; } = null!;
        public SelectList CollectionSelect { get; set; } = null!;

        public class InputModel
        {
            public string Title { get; set; } = null!;
            public int? AuthorId { get; set; }
            public int? CollectionId { get; set; }
            public DateTime? PaintingDate { get; set; }
            public string? Description { get; set; }
        }

        public async Task OnGetAsync()
        {
            AuthorSelect = new SelectList(await _db.Authors.OrderBy(a => a.Name).ToListAsync(), "Id", "Name");
            CollectionSelect = new SelectList(await _db.Collections.OrderBy(c => c.Title).ToListAsync(), "Id", "Title");
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? ImageFile)
        {
            if (string.IsNullOrWhiteSpace(Input.Title))
            {
                ModelState.AddModelError(nameof(Input.Title), "Введите название");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Выберите файл изображения");
                await OnGetAsync();
                return Page();
            }

            // ensure upload folder exists
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            // save file with unique name
            var ext = Path.GetExtension(ImageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(uploads, fileName);
            using (var stream = System.IO.File.Create(path))
            {
                await ImageFile.CopyToAsync(stream);
            }

            var image = new ImageItem
            {
                Title = Input.Title,
                AuthorId = Input.AuthorId,
                CollectionId = Input.CollectionId,
                Description = Input.Description,
                PaintingDate = Input.PaintingDate,
                FileName = fileName,
                CreatedAt = DateTime.UtcNow
            };

            _db.Images.Add(image);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Gallery/Index");
        }
    }
}
