using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class BaseOrderItem : BaseClass
    {
        public BaseOrderItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string? Id { get; set; }
        public string? ProductId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public string? OrderId { get; set; }
    }
    public class OrderItem : BaseOrderItem
    {
        [ForeignKey(nameof(ProductId))]
        public Product? product { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? order { get; set; }
    }
}
