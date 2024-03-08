using Ecommerce.Models;
using Ecommerce.ViewModel.Product;
using System.ComponentModel;

namespace Ecommerce.ViewModel.Cart
{
    public class CartItemViewModel : BaseCartItem
    {
        public ProductViewModel? Product { get; set; }
    }
}
