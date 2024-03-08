using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Order;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class OrderItemRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public OrderItemRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IQueryable<OrderItem> GetItem()
        {
            return _context.Set<OrderItem>();
        }
        public void Add(OrderItemCrudModel cartitem)
        {
            var data = _mapper.Map<OrderItem>(cartitem);
            _context.Set<OrderItem>().Add(data);
        }
        public void Update(OrderItemCrudModel cartitem)
        {
            var data = _mapper.Map<OrderItem>(cartitem);
            _context.Set<OrderItem>().Update(data);
        }
        public void Delete(OrderItem cartitem)
        {
            //var data = _mapper.Map<CartItem>(cartitem);
            _context.Set<OrderItem>().Remove(cartitem);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public OrderItem FirstOrDefault(Expression<Func<OrderItem, bool>> model)
        {
            IQueryable<OrderItem> data = _context.Set<OrderItem>();
            return data.FirstOrDefault(model);
        }
    }
}
