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
        public string? Email { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]+$", ErrorMessage = "*Password must contain at least one number, and one special characters.")]
        [MaxLength(255, ErrorMessage = "Not over 255 letters")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [MaxLength(100, ErrorMessage = "Not over 100 letters")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [MaxLength(100, ErrorMessage = "Not over 100 letters")]
        public string? LastName { get; set; }
        public string? CartId { get; set; }
        [Required(ErrorMessage = "You must enter this field")]
        [MaxLength(20)]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public Guid UserGuid { get; set; }
        public int? Gender { get; set; }
        public string? Avatar { get; set; }
        public int? Role { get; set; }
        public bool? AccountActivated { get; set; }
        public Guid ResetPasswordGuid { get; set; }
        public Guid ActivateToken { get; set; }
    }
    public class User : BaseUser
    {
        [ForeignKey(nameof(CartId))]    
        public Cart? cart { get; set; }
    }
}
