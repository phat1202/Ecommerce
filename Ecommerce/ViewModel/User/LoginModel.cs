using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModel.User
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool Remember { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
