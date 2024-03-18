using CloudinaryDotNet.Actions;
using Ecommerce.Models;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Image;
using Org.BouncyCastle.Bcpg;
using System.Globalization;

namespace Ecommerce.ViewModel.Product
{
    public class ProductViewModel : BaseProduct
    {
        public string? IsActiveStatus
        {
            get
            {
                return IsActive ? "Đang Hoạt Động" : "Tạm Dừng";
            }
        }
        public string? IsDeletedStatus
        {
            get
            {
                return IsDeleted ? "Đã Xóa" : null;
            }
        }
        public string? CreatedAtDisplay
        {
            get
            {
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }
        //public string? PriceDisplay
        //{
        //    get
        //    {            
        //        CultureInfo culture = CultureInfo.GetCultureInfo("vi-VN");
        //        return String.Format(culture, "{0:c}", Price);
        //    }
        //}
        public string? PriceDisplay
        {
            get
            {
                CultureInfo culture = CultureInfo.GetCultureInfo("en-US"); // Change to en-US for dollars
                return string.Format(culture, "{0:c}", Price);
            }
        }
        public string? CategoryNameDisplay { get; set; }
        public string? ProductImageUrl { get; set; }
        public List<ProductImageViewModel>? productImage { get; set; }
        public List<ImageViewModel>? ListProductImage { get; set; }
    }
}
