using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Product;

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
        public void Delete(ProductImageCrudModel image)
        {
            var data = _mapper.Map<ProductImage>(image);
            _context.Set<ProductImage>().Remove(data);
        }
    }
}
