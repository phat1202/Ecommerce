using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Product;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;

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
        public IQueryable<Product> GetItem()
        {
            return _context.Set<Product>();
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
        public Product FirstOrDefault(Expression<Func<Product, bool>> model)
        {
            IQueryable<Product> data = _context.Set<Product>();
            return data.FirstOrDefault(model);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
