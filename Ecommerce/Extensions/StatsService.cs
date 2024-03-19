using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Product;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Ecommerce.Extensions
{
    public class StatsService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public StatsService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public List<Category> GetAllCategories()
        {
            return _context.Set<Category>().OrderBy(item => item.Name).ToList();
        }
        public CategoryViewModel GetCategoryByProduct(string productId)
        {
            var product = _context.Set<Product>().First(p => p.ProductId == productId);
            var data = _context.Set<Category>().First(c => c.CategoryId == product.CategoryId);
            var result = _mapper.Map<CategoryViewModel>(data);
            return result;
        }
        public ProductViewModel GetProduct(string productId)
        {
            var data = _context.Set<Product>().Find(productId);
            if (data == null)
            {
                return null;
            }
            var result = _mapper.Map<ProductViewModel>(data);
            return result;
        }
        public List<ProductViewModel> GetAllProductByCategory(string categoryId)
        {
            var products = _context.Set<Product>().Where(p => p.CategoryId == categoryId)
                .Include(c => c.Category).ToList();
            var result = _mapper.Map<List<ProductViewModel>>(products);
            foreach (var item in result)
            {
                var productImage = _context.Set<ProductImage>().Where(p => p.ProductId == item.ProductId)
                    .Include(i => i.image).First();
                item.ProductImageUrl = productImage.image.ImageUrl;
            }
            return result;
        }
        public string ConvertIntoViewFormat(decimal? money)
        {
            //CultureInfo culture = CultureInfo.GetCultureInfo("vi-VN");
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            return string.Format(culture, "{0:c}", money);
        }
    }
}
