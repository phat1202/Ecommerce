using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Product;

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
        public ProductViewModel GetProduct(string productId)
        {
            var data =  _context.Set<Product>().Find(productId);
            if (data == null)
            {
                return null;
            }
            var result = _mapper.Map<ProductViewModel>(data);
            return result;
        }
    }
}
