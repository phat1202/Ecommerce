using Ecommerce.Models;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Image;
using Org.BouncyCastle.Bcpg;

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
        public string? CategoryNameDisplay { get; set; }
        public List<ProductImageViewModel>? productImage { get; set; }
        public List<ImageViewModel>? ListProductImage { get; set; }
    }
}
