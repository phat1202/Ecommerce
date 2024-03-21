using AutoMapper;
using Ecommerce.Const;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Order;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers
{
    public class ManagerController : Controller
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
        public ManagerController(EcommerceDbContext context, IMapper mapper, IWebHostEnvironment env)
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

            var orders = _orderRepo.GetItem().ToList();
            var result = _mapper.Map<List<OrderViewModel>>(orders);
            return View(result);
        }

        public IActionResult OrderStatusManager(string orderId)
        {
            var order = _orderRepo.GetItem()
                                    .Include(i => i.user)
                                    .Include(i => i.OrderItems).ThenInclude(i => i.product)
                                    .Where(i => i.OrderId == orderId).FirstOrDefault();
            var result = _mapper.Map<OrderCrudModel>(order);


            List<SelectListItem> statusList = Enum.GetValues(typeof(EnumClass.OrderStatus))
                                                         .Cast<EnumClass.OrderStatus>()
                                                         .Select(x => new SelectListItem
                                                         {
                                                             Value = ((int)x).ToString(),
                                                             Text = GetEnumDisplayName(x)
                                                         })
                                                         .ToList();

            ViewData["OrderStatus"] = new SelectList(statusList, "Value", "Text");
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> OrderStatusManager(OrderCrudModel model)
        {
            var order = _orderRepo.FirstOrDefault(x => x.OrderId == model.OrderId);
            if (order == null)
            {
                return NotFound("Error");
            }
            order.OrderStatus = model.OrderStatus;
            await _orderRepo.CommitAsync();
            return View();
        }
        static string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var displayAttribute = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            return displayAttribute != null ? displayAttribute.Name : value.ToString();
        }
    }
}
