using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class BaseProductImage : BaseClass
    {
        public BaseProductImage()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string? Id { get; set; }
        public string? ImageId { get; set; }
        public string? ProductId { get; set; }
    }
    public class ProductImage : BaseProductImage
    {
        [ForeignKey(nameof(ProductId))]
        public Product? product { get; set; }
        [ForeignKey(nameof(ImageId))]
        public Image? image { get; set; }
    }
}
