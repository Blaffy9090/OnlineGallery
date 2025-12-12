using Microsoft.AspNetCore.Identity;

namespace OnlineGallery.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string? DisplayName { get; set; }

        public List<Like> Likes { get; set; } = new();

    }
}
