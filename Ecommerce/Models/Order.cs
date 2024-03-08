using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class BaseOrder : BaseClass
    {
        public BaseOrder()
        {
            OrderId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? OrderId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? OrderTitle { get; set; }

        [Required]
        [MaxLength(255)]
        public string? OrderCode { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Address { get; set; }
        [Required]
        public int? OrderStatus { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? UserId { get; set; }

    }
    public class Order : BaseOrder
    {
        [ForeignKey(nameof(UserId))]
        public User? user { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
