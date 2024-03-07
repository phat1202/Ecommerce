using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task <IActionResult> AddItemToCart(string productId)
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
                if(checkItemExist == null)
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
        //[HttpPost]
        //public async Task<IActionResult> UpdateCartItem(string productId, int? quantity, string action)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
        //        var userId = HttpContext.User.Claims.First().Value;
        //        var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
        //        var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
        //        var quantityItemUpdate = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId && i.CartId == cartUser.CartId);
        //        switch (action)
        //        {
        //            case "plus":
        //                quantityItemUpdate.Quantity = quantity ?? quantityItemUpdate.Quantity++;
        //                break;
        //            case "minus":
        //                if (quantity == null)
        //                {
        //                    if (quantityItemUpdate.Quantity == 0 )
        //                    {
        //                        var data = _mapper.Map<CartItemCrudModel>(quantityItemUpdate);
        //                        _cartItemRepo.Delete(data);
        //                    }
        //                    else
        //                    {
        //                        quantityItemUpdate.Quantity--;
        //                    }
        //                }
        //                else
        //                {
        //                    quantityItemUpdate.Quantity = quantity;
        //                }
        //                break;
        //            default:
        //                return RedirectToAction("Index");
        //        }
        //        await _productRepo.CommitAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Login", "User");
        //}
        [HttpPost]
        public IActionResult DeleteCartItem(string productId)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> PlusCartItemAsync(string productId)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
            var userId = HttpContext.User.Claims.First().Value;
            var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
            var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
            var quantityItemUpdate = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId && i.CartId == cartUser.CartId);
            quantityItemUpdate.Quantity++;
            await _cartItemRepo.CommitAsync();
            return Ok();
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
                _cartItemRepo.Delete(quantityItemUpdate);
            }
            else
            {
                quantityItemUpdate.Quantity--;
            }
            await _cartItemRepo.CommitAsync();
            return Ok();
        }
    }
}
