using Ecommerce.Const;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Order;

namespace Ecommerce.ViewModel.User
{
    public class UserViewModel : BaseUser
    {
        public string? GenderDisplay => Gender != null ? Enum.GetName(typeof(EnumClass.Gender), Gender) : null;

        //public string? GenderDisplay => Enum.GetName(typeof(EnumClass.Gender), this.Gender);
        //public string RoleDisplay => Enum.GetName(typeof(EnumClass.Role), this.Role);
        public string? RoleDisplay => Role != null ? Enum.GetName(typeof(EnumClass.Role), Role) : null;

        public CartViewModel? Cart { get; set; }
        public OrderViewModel? OrderViewModel { get; set; }
    }
}
