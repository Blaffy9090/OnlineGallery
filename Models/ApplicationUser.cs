using Microsoft.AspNetCore.Identity;

namespace OnlineGallery.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add profile fields later (DisplayName, About, AvatarPath, etc.)
        public string? DisplayName { get; set; }
    }
}
