using Ecommerce.Models;

namespace Ecommerce.Extensions
{
    public class StatsService
    {
        private readonly EcommerceDbContext _context;
        public StatsService(EcommerceDbContext context)
        {
            _context = context;
        }
        public List<Category> GetAllCategories()
        {
            return _context.Set<Category>().ToList();
        }
    }
}
