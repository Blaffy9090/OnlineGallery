using System.ComponentModel.DataAnnotations;

namespace OnlineGallery.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }

        public virtual ICollection<ImageItem> Images { get; set; } = new List<ImageItem>();
    }
}
