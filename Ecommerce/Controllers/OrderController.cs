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
        //public async Task<IActionResult> GeneratePDF(string orderId)
        //{
        //    List<string> listProductName = new List<string>();
        //    var order = _orderRepo
        //      .GetItem().Where(x => x.OrderId == orderId)
        //      .Include(x => x.user)
        //      .Include(x => x.OrderItems)
        //        .ThenInclude(x => x.product)
        //      .FirstOrDefault();
        //    if (order != null)
        //    {
        //        var user = _userRepo.GetById(order.user.UserId);
        //        if (order.OrderItems != null)
        //        {
        //            foreach (var orderItem in order.OrderItems)
        //            {
        //                var products = _productRepo.FirstOrDefault(x => x.ProductId == orderItem.ProductId);
        //                if (products != null)
        //                {
        //                    listProductName.Add(products.ProductName);
        //                }
        //            }
        //        }
        //        var productNameResult = String.Join(", ", listProductName.ToArray());
        //        var webRoot = _env.WebRootPath;
        //        var orderTemplate = Path.Combine(webRoot, "template/invoice.html");
        //        var orderTemplateBody = System.IO.File.ReadAllText(orderTemplate);
        //        orderTemplateBody = orderTemplateBody
        //            .Replace("{{orderCode}}", order.OrderCode!.ToString())
        //            .Replace("{{userName}}", user.Name)
        //            .Replace("{{address}}", order.Address)
        //            .Replace("{{productName}}", productNameResult);
        //        var renderer = new HtmlToPdf();
        //        renderer.RenderHtmlAsPdf(orderTemplateBody).SaveAs("Order.pdf");

        //    }
        //    return RedirectToAction("Index");
        //}

    }
}
