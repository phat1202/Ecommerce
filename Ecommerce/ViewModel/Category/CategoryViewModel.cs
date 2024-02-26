using Ecommerce.Const;
using Ecommerce.Models;

namespace Ecommerce.ViewModel.Category
{
    public class CategoryViewModel : BaseCategory
    {
        public string? IsActiveStatus
        {
            get
            {
                return IsActive ? "Đang Hoạt Động" : "Tạm Dừng";
            }
        }
        public string? CreatedAtDisplay
        {
            get
            {
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }
        //public string? IsDeletedStatus
        //{
        //    get
        //    {
        //        return IsDelete ? "Xoa" ? null;
        //    }
        //}
    }
}
