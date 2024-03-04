using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Product;

namespace Ecommerce.Repositories
{
    public class ImageRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ImageRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IQueryable<Image> GetItem()
        {
            return _context.Set<Image>();
        }
        public void Add(ImageCrudModel image)
        {
            var data = _mapper.Map<Image>(image);
            _context.Set<Image>().Add(data);
        }
        public void Update(ImageCrudModel image)
        {
            var data = _mapper.Map<Image>(image);
            _context.Set<Image>().Update(data);
        }
        public void Delete(ImageCrudModel image)
        {
            var data = _mapper.Map<Image>(image);
            _context.Set<Image>().Remove(data);
        }
    }
}
