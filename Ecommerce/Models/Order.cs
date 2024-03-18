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
        public int? OrderStatus { get; set; }
        public decimal? TotalPrice { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [MaxLength(100, ErrorMessage = "Too Long")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [MaxLength(100, ErrorMessage = "Too Long")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "You must enter this field"), EmailAddress]
        [MaxLength(255, ErrorMessage = "Too Long")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [MaxLength(20)]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        public string? Country { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        public string? City { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        public string? State { get; set; }
        public string? UserId { get; set; }

    }
    public class Order : BaseOrder
    {
        [ForeignKey(nameof(UserId))]
        public User? user { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
