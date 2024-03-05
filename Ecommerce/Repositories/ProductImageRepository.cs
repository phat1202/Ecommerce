using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Product;
using Ecommerce.ViewModel.User;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class ProductImageRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductImageRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IQueryable<ProductImage> GetItem()
        {
            return _context.Set<ProductImage>();
        }

        public void Add(ProductImageCrudModel image)
        {
            var data = _mapper.Map<ProductImage>(image);
            _context.Set<ProductImage>().Add(data);
        }
        public void Update(ProductImageCrudModel image)
        {
            var data = _mapper.Map<ProductImage>(image);
            _context.Set<ProductImage>().Update(data);
        }
        public void Delete(ProductImage image)
        {       
            _context.Set<ProductImage>().Remove(image);
        }
        public ProductImage FirstOrDefault(Expression<Func<ProductImage, bool>> model)
        {
            IQueryable<ProductImage> data = _context.Set<ProductImage>();
            return data.FirstOrDefault(model);
        }
    }
}
