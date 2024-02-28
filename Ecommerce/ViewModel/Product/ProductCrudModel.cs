using Ecommerce.Models;
using Ecommerce.ViewModel.Category;

namespace Ecommerce.ViewModel.Product
{
    public class ProductCrudModel : BaseProduct
    {
        public IFormFile? FileImage { get; set; }
        //public List<CategoryViewModel>? ListCategoryViewModel { get; set; }
       
    }
}
