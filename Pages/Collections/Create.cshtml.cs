using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Collections
{
    [Authorize(Roles = "Moderator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public CreateModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public InputModel Input { get; set; } = new();
        [BindProperty]
        public List<int> SelectedImageIds { get; set; } = new();

        public List<ImageItem> AllImages { get; set; } = new();

        public class InputModel
        {
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
        }

        public async Task OnGetAsync()
        {
            AllImages = await _db.Images.Include(i => i.Author).Where(i => i.CollectionId == null).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Title))
                ModelState.AddModelError(nameof(Input.Title), "¬ведите название");

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var collection = new Collection
            {
                Title = Input.Title,
                Description = Input.Description
            };

            _db.Collections.Add(collection);
            await _db.SaveChangesAsync();

            // assign selected images
            if (SelectedImageIds.Any())
            {
                var imgs = await _db.Images.Where(i => SelectedImageIds.Contains(i.Id)).ToListAsync();
                foreach (var img in imgs)
                    img.CollectionId = collection.Id;
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
