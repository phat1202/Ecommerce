using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class BaseImage : BaseClass
    {
        public BaseImage()
        {
            ImageId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? ImageId { get; set; }
        public string? ImageUrl { get; set; }

    }
    public class Image : BaseImage
    {

    }
}
