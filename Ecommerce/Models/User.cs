using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class BaseUser : BaseClass
    {
        public BaseUser()
        {
            UserId = Guid.NewGuid().ToString();
        }
        [Key]
        public string? UserId { get; set; }
        [Required]
        public string? Email {  get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [DataType(DataType.Password)]
        [MaxLength(255, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(100, ErrorMessage = "Không được vượt quá 255 ký tự")]
        public string? Name { get; set; }
        public Guid UserGuid { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(20)]
        public string? Phone { get; set; }
        public int? Gender { get; set; }
        public string? Avatar { get; set; }
        public int? Role { get; set; }
        public bool? AccountActivated { get; set; }
        public Guid ResetPasswordGuid { get; set; }
        public string? CartId { get; set; }
        public string? Address { get; set; }
    }
    public class User : BaseUser
    {
        [ForeignKey(nameof(CartId))]    
        public Cart? cart { get; set; }
    }
}
