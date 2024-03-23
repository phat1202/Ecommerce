using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModel.User
{
    public class ResetPasswordModel
    {

        [Required]
        public string? UserId { get; set; }
        [Required,MaxLength(125, ErrorMessage ="Password is too long")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()-=;'"":[]{}\|<>/?.,`~]+$", ErrorMessage = "*Password must contain at least one number, and one special characters.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
