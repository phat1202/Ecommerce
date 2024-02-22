using Ecommerce.Const;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;

namespace Ecommerce.ViewModel.User
{
    public class UserViewModel : BaseUser
    {
        public string? GenderDisplay => Enum.GetName(typeof(EnumClass.Gender), this.Gender);
        public string? RoleDisplay => Enum.GetName(typeof(EnumClass.Role), this.Role);
        public CartViewModel? Cart { get; set; }
    }
}
