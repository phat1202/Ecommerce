using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Product;
using System.Globalization;

namespace Ecommerce.ViewModel.Order
{
    public class OrderItemViewModel : BaseOrderItem
    {
        public ProductViewModel? product { get; set; }
        public string? TotalPriceOfProduct
        {
            get
            {
                CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                return string.Format(culture, "{0:c}", Price);
            }
        }
    }
}

