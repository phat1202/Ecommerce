using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Product;

namespace Ecommerce.ViewModel.Order
{
    public class OrderItemViewModel : BaseOrderItem
    {
        public ProductViewModel? product  { get; set;}
    }
}
