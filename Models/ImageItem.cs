using System.ComponentModel.DataAnnotations;

namespace OnlineGallery.Models
{
    public class ImageItem
    {
        public int Id { get; set; }

        [Required, StringLength(300)]
        public string Title { get; set; } = null!;

        // file name in wwwroot/uploads
        [Required]
        public string FileName { get; set; } = null!;

        // Date the painting was created (semantic painting creation date)
        [DataType(DataType.Date)]
        public DateTime? PaintingDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }

        public int? CollectionId { get; set; }
        public Collection? Collection { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
