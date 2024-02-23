using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModel.User
{
    public class UserCrudModel : BaseUser
    {
        public IFormFile? FileImage { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không đúng")]
        public string? ConfirmPassword { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
