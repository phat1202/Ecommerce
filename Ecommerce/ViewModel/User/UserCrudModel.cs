using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModel.User
{
    public class UserCrudModel : BaseUser
    {
       //public IFormFile? FileImage { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
