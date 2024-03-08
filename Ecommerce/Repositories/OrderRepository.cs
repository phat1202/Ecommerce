using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Order;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class OrderRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public OrderRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IQueryable<Order> GetItem()
        {
            return _context.Set<Order>();
        }
        public void Add(OrderCrudModel cartitem)
        {
            var data = _mapper.Map<Order>(cartitem);
            _context.Set<Order>().Add(data);
        }
        public void Update(OrderCrudModel cartitem)
        {
            var data = _mapper.Map<Order>(cartitem);
            _context.Set<Order>().Update(data);
        }
        public void Delete(Order cartitem)
        {
            //var data = _mapper.Map<CartItem>(cartitem);
            _context.Set<Order>().Remove(cartitem);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public Order FirstOrDefault(Expression<Func<Order, bool>> model)
        {
            IQueryable<Order> data = _context.Set<Order>();
            return data.FirstOrDefault(model);
        }
    }
}
