using Ecommerce.Models;

namespace Ecommerce.ViewModel.Image
{
    public class ImageViewModel : BaseImage
    {
        public string? ProductId { get; set; }
        public string? CreatedAtDisplay
        {
            get
            {
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }
    }
}
