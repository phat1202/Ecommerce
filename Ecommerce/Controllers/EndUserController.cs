using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Order;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class EndUserController : Controller
    {
        private readonly ProductRepository _productRepo;
        private readonly CategoryRepository _categoryRepo;
        private readonly ImageRepository _imageRepo;
        private readonly ProductImageRepository _productImageRepo;
        private readonly UserRepository _userRepo;
        private readonly CartRepository _cartRepo;
        private readonly EcommerceDbContext _context;
        private readonly CartItemRepository _cartItemRepo;
        private readonly OrderRepository _orderRepo;
        private readonly OrderItemRepository _orderItemRepo;
        private readonly IMapper _mapper;
        public EndUserController(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _productRepo = new ProductRepository(_context, _mapper);
            _categoryRepo = new CategoryRepository(_context, _mapper);
            _imageRepo = new ImageRepository(_context, _mapper);
            _productImageRepo = new ProductImageRepository(_context, _mapper);
            _userRepo = new UserRepository(_context, _mapper);
            _cartRepo = new CartRepository(_context, _mapper);
            _cartItemRepo = new CartItemRepository(_context, _mapper);
            _orderRepo = new OrderRepository(_context, _mapper);
            _orderItemRepo = new OrderItemRepository(_context, _mapper);
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult HistoryOrder()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var cartItems = _cartItemRepo.GetItem().Where(i => i.CartId == user.CartId).Include(c => c.cart).ToList();
                var orders = _orderRepo.GetItem()
                    .Include(i => i.user)
                    .Include(i => i.OrderItems).ThenInclude(i => i.product)
                    .Where(o => o.UserId == userId).ToList();

                var ordersViewModel = _mapper.Map<List<OrderViewModel>>(orders);
                foreach (var i in ordersViewModel)
                {
                    foreach (var p in i.OrderItems)
                    {
                        var image = _productImageRepo.GetItem().Where(x => x.ProductId == p.ProductId).Include(i => i.image).First().image.ImageUrl;
                        p.product.ProductImageUrl = image;
                    }
                }
                return View(ordersViewModel);
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult DetailOrder(string orderId)
        {
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
            var cartItems = _cartItemRepo.GetItem().Where(i => i.CartId == user.CartId).Include(c => c.cart).ToList();
            var orders = _orderRepo.GetItem()
                .Include(i => i.user)
                .Include(i => i.OrderItems).ThenInclude(i => i.product)
                .Where(o => o.OrderId == orderId).First();
            var ordersViewModel = _mapper.Map<OrderViewModel>(orders);
            var userView = _mapper.Map<UserViewModel>(user);
            ordersViewModel.Customer = userView;
            return View(ordersViewModel);
        }
    }
}
