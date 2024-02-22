using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModel.User
{
    public class UserCrudModel : BaseUser
    {
        public IFormFile? FileImage { get; set; }
        //[Required]
        //[Compare("Password")]
        //public string? ConfirmPassword { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
