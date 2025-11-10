using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Users
{
    [Authorize(Roles = "Moderator")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public DeleteModel(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        [BindProperty] public ApplicationUser UserData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            UserData = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);

            return RedirectToPage("Index");
        }
    }
}
