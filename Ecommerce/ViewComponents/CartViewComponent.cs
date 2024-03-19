using AutoMapper;
using Ecommerce.Const;
using Ecommerce.Extensions;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Ecommerce.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly UserRepository _userRepo;
        private readonly CartRepository _cartRepo;
        private readonly CartItemRepository _cartItemRepo;
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public CartViewComponent(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _userRepo = new UserRepository(_context, _mapper);
            _cartRepo = new CartRepository(_context, _mapper);
            _cartItemRepo = new CartItemRepository(_context, _mapper); ;
        }
        public IViewComponentResult Invoke()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId).CartItems;
                var cartItems = _cartItemRepo.GetItem().Include(c => c.cart).Where(i => i.CartId == user.CartId).ToList();
                var cart = _mapper.Map<List<CartItemViewModel>>(cartItems);
                return View("CartPanel", new CartPanelViewModel
                {
                    TotalQuantity = cart.Select(i => i.CartItemId).Count(),
                });
            }
            else
            {
                var cart = HttpContext.Session.Get<List<CartItemViewModel>>(MyConst.CartKey) ?? new List<CartItemViewModel>();
                return View("CartPanel", new CartPanelViewModel
                {
                    TotalQuantity = cart.Select(i => i.CartItemId).Count(),
                });
            }

        }
    }
}
