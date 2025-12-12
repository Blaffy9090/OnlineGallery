using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Security.Claims;

namespace OnlineGallery.Pages.Images
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public ImageItem Image { get; set; } = null!;
        public List<ImageTag> Tags { get; set; } = new();
        public int LikeCount { get; set; }
        public bool UserLiked { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var img = await _db.Images
                .Include(i => i.Author)
                .Include(i => i.Collection)
                .Include(i => i.Tags)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (img == null) return NotFound();

            Image = img;
            Tags = img.Tags;

            LikeCount = await _db.Likes.CountAsync(l => l.ImageId == id);

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                var user = await _userManager.FindByIdAsync(userId);

                UserLiked = await _db.Likes.AnyAsync(l => l.ImageId == id && l.UserId == user!.Id);
            }

            return Page();
        }

        [Authorize]
        public async Task<JsonResult> OnPostToggleLikeAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return new JsonResult(new { success = false }) { StatusCode = 401 };

            var like = await _db.Likes.FirstOrDefaultAsync(l => l.ImageId == id && l.UserId == user.Id);
            bool likedNow;
            if (like != null)
            {
                _db.Likes.Remove(like);
                likedNow = false;
            }
            else
            {
                _db.Likes.Add(new Like { ImageId = id, UserId = user.Id });
                likedNow = true;
            }
            await _db.SaveChangesAsync();

            var likeCount = await _db.Likes.CountAsync(l => l.ImageId == id);
            return new JsonResult(new { likeCount, likedByUser = likedNow });
        }

        [Authorize(Roles = "Moderator")]
        public async Task<JsonResult> OnPostAddTagAsync(int id, [FromBody] TagInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
                return new JsonResult(new { success = false, error = "Пустой тег" });

            bool exists = await _db.Tags.AnyAsync(t => t.ImageId == id && t.Name == input.Name);
            if (exists) return new JsonResult(new { success = false, error = "Такой тег уже существует" });

            var tag = new ImageTag { ImageId = id, Name = input.Name };
            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public class TagInput
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
