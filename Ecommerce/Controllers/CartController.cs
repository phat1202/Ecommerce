using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Order;
using Ecommerce.ViewModel.Product;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
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
        public CartController(EcommerceDbContext context, IMapper mapper)
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
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var cartItems = _cartItemRepo.GetItem().Where(i => i.CartId == user.CartId).Include(c => c.cart).ToList();
                var result = _mapper.Map<List<CartItemViewModel>>(cartItems);
                foreach (var item in result)
                {
                    var product = _productRepo.FirstOrDefault(p => p.ProductId == item.ProductId);
                    var productViewModel = _mapper.Map<ProductViewModel>(product);
                    var productImage = _productImageRepo.GetItem().Where(p => p.ProductId == product.ProductId)
                                        .Include(i => i.image).First();
                    //var images = _imageRepo.FirstOrDefault(i => i.ImageId == productImage.ImageId);
                    productViewModel.ProductImageUrl = productImage.image.ImageUrl;
                    item.Product = productViewModel;
                }
                result.OrderBy(i => i.CreatedAt);
                return View(result);
            }
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public async Task<IActionResult> AddItemToCart(string productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
                if (product == null || product.Quantity == 0)
                {
                    return Json(new { success = false, loginError = false, message = "Sản phẩm đã hết hàng" });
                }
                var checkItemExist = _cartItemRepo.FirstOrDefault(i => i.ProductId == productId && i.CartId == cartUser.CartId);
                if (checkItemExist == null)
                {
                    var newItem = new CartItemCrudModel
                    {
                        CartId = cartUser.CartId,
                        ProductId = product.ProductId,
                        Quantity = 1,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    _cartItemRepo.Add(newItem);
                }
                else
                {
                    checkItemExist.Quantity++;
                }

                await _productRepo.CommitAsync();
                return Json(new { success = true, loginError = false, message = "Sản phẩm đã được thêm vào giỏ hàng." });
            }
            return Json(new { success = false, loginError = true, message = "Bạn cần phải đăng nhập trước." });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCartItemAsync(string productId)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
            var cartItem = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId
                                                          && i.CartId == cartUser.CartId);
            //var result = _mapper.Map<CartItemCrudModel>(cartItem);
            _cartItemRepo.Delete(cartItem);
            await _cartItemRepo.CommitAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> PlusCartItemAsync(string productId)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
            var quantityAvailable = product.Quantity;
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
            var quantityItemUpdate = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId && i.CartId == cartUser.CartId);
            quantityItemUpdate.Quantity++;
            if (quantityItemUpdate.Quantity > quantityAvailable)
            {
                return Json(new { success = false, errorMessage = "Số lượng bạn đặt đã đạt mức tối đa của sản phẩm này" });
            }
            await _cartItemRepo.CommitAsync();
            return Json(new { success = true, errorMessage = "" });
        }
        [HttpPost]
        public async Task<IActionResult> MinusCartItemAsync(string productId)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
            var quantityItemUpdate = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId && i.CartId == cartUser.CartId);

            if (quantityItemUpdate.Quantity == 1)
            {
                //var data = _mapper.Map<CartItemCrudModel>(quantityItemUpdate);
                //_cartItemRepo.Delete(quantityItemUpdate);
                quantityItemUpdate.Quantity = 1;
            }
            else
            {
                quantityItemUpdate.Quantity--;
            }
            await _cartItemRepo.CommitAsync();
            return Ok();
        }
        public IActionResult SelectItem(string itemId)
        {
            var result = _cartItemRepo.FirstOrDefault(i => i.CartItemId == itemId);
            if (result.ItemSelected == true)
            {
                result.ItemSelected = false;
            }
            else
            {
                result.ItemSelected = true;
            }
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult CheckOut()
        {
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
            var ItemOrders = _cartItemRepo.GetItem().Where(i => i.ItemSelected == true && i.CartId == cartUser.CartId)
                .Include(p => p.product)
                .Include(c => c.cart)
                .ToList();
            var userView = _mapper.Map<UserViewModel>(user);
            var cartView = _mapper.Map<CartViewModel>(cartUser);
            var cartItemView = _mapper.Map<List<CartItemViewModel>>(ItemOrders);
            cartView.CartItems = cartItemView;
            userView.Cart = cartView;
            return View(userView);
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(UserViewModel model)
        {
            try
            {


                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var ItemOrders = _cartItemRepo.GetItem().Where(i => i.ItemSelected == true && i.CartId == cartUser.CartId)
                                            .Include(p => p.product).ToList();
                var productsSold = new List<string>();
                decimal SubtotalOrder = 0;
                string AddressDelivery;
                if (model.DeliveryDifferentAddress)
                {
                    AddressDelivery = model.AddressDelivery;
                }
                else
                {
                    AddressDelivery = user.Address;
                }
                foreach (var item in ItemOrders)
                {
                    var price = item.Quantity * item.product.Price;
                    SubtotalOrder += (decimal)price;
                }
                var newOrder = new OrderCrudModel
                {
                    OrderTitle = ItemOrders.FirstOrDefault().product.ProductName,
                    OrderCode = "ORD" + DateTime.Now.Ticks + GetIdCode(userId),
                    CreatedAt = DateTime.Now,
                    OrderStatus = 0,
                    UserId = userId,
                    Address = AddressDelivery,
                    TotalPrice = SubtotalOrder,
                };
                _orderRepo.Add(newOrder);
                await _orderRepo.CommitAsync();
                //Create OrderItem List
                foreach (var item in ItemOrders)
                {
                    var orderItems = new OrderItemCrudModel
                    {
                        ProductId = item.ProductId,
                        Price = item.Quantity * item.product.Price,
                        Quantity = item.Quantity,
                        OrderId = newOrder.OrderId,
                        CreatedAt = DateTime.Now,
                        IsActive = true,
                    };
                    _orderItemRepo.Add(orderItems);
                    await _orderItemRepo.CommitAsync();

                    //Remove CartItem
                    _cartItemRepo.Delete(item);
                    await _cartItemRepo.CommitAsync();
                    //Cập nhật số lượng hàng trong Kho.
                    var product = _productRepo.FirstOrDefault(p => p.ProductId == item.ProductId);
                    product.Quantity = product.Quantity - item.Quantity;
                    await _productRepo.CommitAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Test()
        {
            var order = _context.Orders.First().OrderCode;
            var data = _orderRepo.GetUserByOrderId(order);
            return View();
        }
        private string GetIdCode(string userId)
        {
            string[] parts = userId.Split('-');
            string secondPartWithHyphen = parts[1].ToUpper();
            return secondPartWithHyphen;
        }
    }
}
