using static System.Net.Mime.MediaTypeNames;

namespace OnlineGallery.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int ImageId { get; set; }
        public ImageItem Image { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
