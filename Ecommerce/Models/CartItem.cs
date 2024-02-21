using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class BaseCartItem : BaseClass
    {
        public BaseCartItem()
        {
            CartItemId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? CartItemId { get; set; }
        public int? Quantity { get; set; }
        public string? ProductId { get; set; }
        public string? CartId { get; set; }
    }
    public class CartItem : BaseCartItem
    {
        [ForeignKey(nameof(ProductId))]
        public Product? product { get; set; }
        [ForeignKey(nameof(CartId))]
        public Cart? cart { get; set; }
    }
}
