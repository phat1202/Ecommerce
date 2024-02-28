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
            var data = _mapper.Map<Image>(image);
            _context.Set<Image>().Add(data);
        }
        public void Update(ProductImageCrudModel image)
        {
            var data = _mapper.Map<Image>(image);
            _context.Set<Image>().Update(data);
        }
        public void Delete(ProductImageCrudModel image)
        {
            var data = _mapper.Map<Image>(image);
            _context.Set<Image>().Remove(data);
        }
    }
}
