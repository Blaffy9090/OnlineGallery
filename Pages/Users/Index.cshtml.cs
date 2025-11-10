using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineGallery.Models;

namespace OnlineGallery.Pages.Users
{
    [Authorize(Roles = "Moderator")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IndexModel(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
