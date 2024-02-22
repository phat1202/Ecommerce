using Ecommerce.Models;
using Ecommerce.ViewModel.Product;

namespace Ecommerce.ViewModel.Cart
{
    public class CartItemViewModel : BaseCartItem
    {
        public ProductViewModel? Product { get; set; }
    }
}
