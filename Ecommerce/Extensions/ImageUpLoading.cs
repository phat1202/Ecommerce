using Google.Protobuf.Reflection;
using System.Security.Principal;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Ecommerce.Extensions
{
    public class ImageUpLoading
    {
        public string UploadImage(IFormFile? imageFile)
        {
            if (imageFile == null)
            {
                return null;
            }
            else
            {
                using var stream = imageFile.OpenReadStream();
                var account = new Account("dqnsplymn", "279175116359664", "Oii8kBOmGAaOw_Wadnp0Rwc9oFk");
                var cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, stream)
                };
                var result = cloudinary.Upload(uploadParams);
                var imageUrl = result.SecureUrl.OriginalString.ToString();
                return imageUrl;
            }
        }
        public bool RemoveImageByUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return false;
            }
            var publicId = GetPublicIdFromImageUrl(imageUrl);

            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            var account = new Account("dqnsplymn", "279175116359664", "Oii8kBOmGAaOw_Wadnp0Rwc9oFk");
            var cloudinary = new Cloudinary(account);

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image 
            };

            var result = cloudinary.Destroy(deletionParams);

            return result.Result == "ok";
        }
        private string GetPublicIdFromImageUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;         
            if (segments.Length >= 2)
            {
                return segments[segments.Length - 2].TrimEnd('/');
            }

            return null;
        }
    }
}
