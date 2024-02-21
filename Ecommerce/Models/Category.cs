using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class BaseCategory : BaseClass
    {
        public BaseCategory()
        {
            CategoryId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? CategoryId { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(255, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string? Name { get; set; }
    }
    public class Category : BaseCategory
    {

    }
}
