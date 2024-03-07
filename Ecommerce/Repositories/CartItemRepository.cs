using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class CartItemRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CartItemRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IQueryable<CartItem> GetItem()
        {
            return _context.Set<CartItem>();
        }
        public void Add(CartItemCrudModel cartitem)
        {
            var data = _mapper.Map<CartItem>(cartitem);
            _context.Set<CartItem>().Add(data);
        }
        public void Update(CartItemCrudModel cartitem)
        {
            var data = _mapper.Map<CartItem>(cartitem);
            _context.Set<CartItem>().Update(data);
        }
        public void Delete(CartItem cartitem)
        {
            //var data = _mapper.Map<CartItem>(cartitem);
            _context.Set<CartItem>().Remove(cartitem);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public CartItem FirstOrDefault(Expression<Func<CartItem, bool>> model)
        {
            IQueryable<CartItem> data = _context.Set<CartItem>();
            return data.FirstOrDefault(model);
        }
    }
}
