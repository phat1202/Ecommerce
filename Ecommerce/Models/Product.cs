using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class BaseProduct : BaseClass
    {
        public BaseProduct()
        {
            ProductId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? ProductId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int? Quantity { get; set; }
        [Precision(18, 2)]
        public decimal? Price { get; set; }
        public string? CategoryId { get; set; }
    }
    public class Product : BaseProduct
    {
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
    }
}
