using Ecommerce.Models;
using Ecommerce.ViewModel.User;

namespace Ecommerce.ViewModel.Order
{
    public class OrderViewModel : BaseOrder
    {
        public List<OrderItemViewModel>? OrderItems { get; set; }
        public UserViewModel? Customer { get; set; }
    }
}
