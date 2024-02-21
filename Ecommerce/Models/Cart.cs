using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class BaseCart : BaseClass
    {
        public BaseCart()
        {
            CartId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? CartId { get; set; }
        [Precision(18, 2)]
        public decimal? TotalPrice { get; set; }
    }
    public class Cart : BaseCart
    {
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
