using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Users
{
    [Authorize(Roles = "Moderator")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public ApplicationUser UserData { get; set; } = new();

        [BindProperty]
        public bool IsModerator { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            UserData = user;
            IsModerator = await _userManager.IsInRoleAsync(user, "Moderator");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserData.Id);
            if (user == null) return NotFound();

            // Update basic info
            user.DisplayName = UserData.DisplayName;
            user.Email = UserData.Email;
            user.UserName = UserData.UserName;
            await _userManager.UpdateAsync(user);

            // Ensure the role exists
            if (!await _roleManager.RoleExistsAsync("Moderator"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

            // Assign or remove Moderator role
            if (IsModerator)
            {
                if (!await _userManager.IsInRoleAsync(user, "Moderator"))
                {
                    await _userManager.AddToRoleAsync(user, "Moderator");
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(user, "Moderator"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Moderator");
                }
            }

            return RedirectToPage("Index");
        }
    }
}