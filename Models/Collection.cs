using System.ComponentModel.DataAnnotations;

namespace OnlineGallery.Models
{
    public class Collection
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<ImageItem> Images { get; set; } = new List<ImageItem>();
    }
}
