using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineGallery.Models
{
    public class ImageTag
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        public int ImageId { get; set; }
        public ImageItem Image { get; set; } = null!;

    }
}
