using System.ComponentModel;

namespace Ecommerce.Models
{
    public class BaseClass
    {
        public BaseClass()
        {
            UpdatedAt = DateTime.Now;
            CreatedAt = DateTime.Now;
        }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsDelete { get; set; }
    }
}
