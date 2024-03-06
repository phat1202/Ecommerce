using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.User;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;
using System.Net.WebSockets;
using ZstdSharp.Unsafe;

namespace Ecommerce.Repositories
{
    public class CartRepository
    {   
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CartRepository(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void Add(CartCrudModel cart)
        {
            var data = _mapper.Map<Cart>(cart);
            _context.Set<Cart>().Add(data);
        }
        public void Update(CartCrudModel cart)
        {
            var data = _mapper.Map<Cart>(cart);
            _context.Set<Cart>().Update(data);
        }
        public void Delete(CartCrudModel cart)
        {
            var data = _mapper.Map<Cart>(cart);
            _context.Set<Cart>().Remove(data);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public Cart FirstOrDefault(Expression<Func<Cart, bool>> model)
        {
            IQueryable<Cart> data = _context.Set<Cart>();
            return data.FirstOrDefault(model);
        }
    }
}
