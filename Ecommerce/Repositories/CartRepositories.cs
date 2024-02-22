using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Net.WebSockets;
using ZstdSharp.Unsafe;

namespace Ecommerce.Repositories
{
    public class CartRepositories
    {   
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CartRepositories(EcommerceDbContext context, IMapper mapper)
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
    }
}
