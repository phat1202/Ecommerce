using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Product;
using System.Linq.Expressions;

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
        public void Delete(Image image)
        {
            _context.Set<Image>().Remove(image);
        }
        public Image FirstOrDefault(Expression<Func<Image, bool>> model)
        {
            IQueryable<Image> data = _context.Set<Image>();
            return data.FirstOrDefault(model);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
