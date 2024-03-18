using Ecommerce.Const;
using Ecommerce.Models;
using Ecommerce.ViewModel.User;
using System.Globalization;

namespace Ecommerce.ViewModel.Order
{
    public class OrderViewModel : BaseOrder
    {
        public List<OrderItemViewModel>? OrderItems { get; set; }
        public UserViewModel? Customer { get; set; }
        public string? StatusOrder => OrderStatus != null ? Enum.GetName(typeof(EnumClass.OrderStatus), OrderStatus) : null;
        public string? CreatedAtDisplay
        {
            get
            {
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }
        public string? TotalPriceDisplay
        {
            get
            {
                CultureInfo culture = CultureInfo.GetCultureInfo("en-US"); 
                return string.Format(culture, "{0:c}", TotalPrice);
            }
        }
    }
}
