using Ecommerce.Models;

namespace Ecommerce.ViewModel.Product
{
    public class ProductImageCrudModel : BaseProductImage
    {
        public IFormFile? FileImage { get; set; }
    }
}
