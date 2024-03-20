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
        [MaxLength(255, ErrorMessage = "Not over 255 letters")]
        public string? Name { get; set; }
        public string? CategoryImageUrl { get; set; }
    }
    public class Category : BaseCategory
    {

    }
}
