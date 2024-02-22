using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Category;

namespace Ecommerce.Repositories
{
    public class CategoryRepositories
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CategoryRepositories(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public CategoryCrudModel GetById(string id)
        {
            var data = _context.Set<Category>().Find(id);
            var result = _mapper.Map<CategoryCrudModel>(data);
            return result;
        }
        public List<Category> GetAll()
        {
            return _context.Set<Category>().ToList();
        }
        public void Add(CategoryCrudModel category) 
        { 
            var data = _mapper.Map<Category>(category);
            _context.Set<Category>().Add(data);
        }
        public void Udate(CategoryCrudModel category)
        {
            var data = _mapper.Map<Category>(category);
            _context.Set<Category>().Update(data);
        }
        public void Delete(CategoryCrudModel category)
        {
            var data = _mapper.Map<Category>(category);
            _context.Set<Category>().Remove(data);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
