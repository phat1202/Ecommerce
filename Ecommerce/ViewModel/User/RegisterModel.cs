using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ecommerce.ViewModel.User
{
    public class RegisterModel
    {
        [Required]
        public string? Name { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không đúng")]
        public string? ConfirmPassword { get; set; }
        [Required]
        public int? Gender { get; set; }
        public string? ErrorMessage { get; set; }

    }
}
