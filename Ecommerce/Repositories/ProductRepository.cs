using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Product;

namespace Ecommerce.Repositories
{
    public class ProductRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public ProductCrudModel GetById(string id)
        {
            var product = _context.Set<Product>().Find(id);
            var result = _mapper.Map<ProductCrudModel>(product);
            return result;
        }
        public List<Product> GetAll()
        {
            return _context.Set<Product>().ToList();
        }
        public void Add(ProductCrudModel product)
        {
            var data = _mapper.Map<Product>(product);
            _context.Set<Product>().Add(data);
        }
        public void Update(ProductCrudModel product)
        {
            var data = _mapper.Map<Product>(product);
            _context.Set<Product>().Update(data);
        }
        public void Delete(ProductCrudModel product)
        {
            var data = _mapper.Map<Product>(product);
            _context.Set<Product>().Remove(data);
        }
    }
}
