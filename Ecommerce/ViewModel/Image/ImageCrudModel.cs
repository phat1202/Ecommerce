using Ecommerce.Models;

namespace Ecommerce.ViewModel.Image
{
    public class ImageCrudModel : BaseImage
    {
        public IFormFile? ImageFile { get; set; }
    }
}
