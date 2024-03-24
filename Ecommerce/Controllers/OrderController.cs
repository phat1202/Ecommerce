using AutoMapper;
using Ecommerce.Const;
using Ecommerce.Extensions;
using Ecommerce.Helpers;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Order;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Controllers
{
    public class OrderController : Controller
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
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public OrderController(EcommerceDbContext context, IMapper mapper, IWebHostEnvironment env)
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
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult HistoryOrder()
        {
            var result = new List<OrderViewModel>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var cartItems = _cartItemRepo.GetItem().Where(i => i.CartId == user.CartId).Include(c => c.cart).ToList();
                var orders = _orderRepo.GetItem()
                    .Include(i => i.user)
                    .Include(i => i.OrderItems).ThenInclude(i => i.product)
                    .Where(o => o.UserId == userId).ToList().OrderByDescending(i => i.CreatedAt);

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
            else
            {
                //var data = HttpContext.Session.Get<OrderViewModel>(MyConst.OrderView);
                //result.Add(data);
                return View(result);
            }
        }
        public IActionResult DetailOrder(string orderId)
        {
            try
            {
                //var userId = HttpContext.User.Claims.First().Value;
                //var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                //var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                //var cartItems = _cartItemRepo.GetItem().Where(i => i.CartId == user.CartId).Include(c => c.cart).ToList();
                var orders = _orderRepo.GetItem()
                    .Include(i => i.user)
                    .Include(i => i.OrderItems).ThenInclude(i => i.product)
                    .Where(o => o.OrderId == orderId).First();
                var ordersViewModel = _mapper.Map<OrderViewModel>(orders);
                //var userView = _mapper.Map<UserViewModel>(user);
                //ordersViewModel.Customer = userView;
                return View(ordersViewModel);
            }
            catch (Exception)
            {

                return RedirectToAction("HistoryOrder");
            }

        }

    }
}
