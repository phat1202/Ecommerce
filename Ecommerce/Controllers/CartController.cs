using AutoMapper;
using CloudinaryDotNet;
using Ecommerce.Const;
using Ecommerce.Extensions;
using Ecommerce.Helpers;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.Order;
using Ecommerce.ViewModel.Product;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using PayPal.Api;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static Ecommerce.Const.EnumClass;
using Amount = PayPal.Api.Amount;
using Payer = PayPal.Api.Payer;

namespace Ecommerce.Controllers
{

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
        private readonly PaypalClient _paypalClient;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;
        public CartController(EcommerceDbContext context, IMapper mapper, PaypalClient paypalClient, IHttpContextAccessor httpContextAccessor, IConfiguration iconfiguration)
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
            _paypalClient = paypalClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = iconfiguration;
        }
        public List<CartItemViewModel> Cart => HttpContext.Session.Get<List<CartItemViewModel>>(MyConst.CartKey)
                                                ?? new List<CartItemViewModel>();
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
            var carts = HttpContext.Session.Get<List<CartItemViewModel>>(MyConst.CartKey);
            return View(Cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddItemToCart(string productId, int? quantityInput)
        {
            int quantityOrder = 1;
            if (quantityInput.HasValue && quantityInput.Value > 0)
            {
                quantityOrder = quantityInput.Value;
            }
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
                if (product == null || product.Quantity == 0)
                {
                    return Json(new { success = false, loginError = false, message = "Out Of Stock" });
                }
                var checkItemExist = _cartItemRepo.FirstOrDefault(i => i.ProductId == productId && i.CartId == cartUser.CartId);
                if (checkItemExist == null)
                {
                    var newItem = new CartItemCrudModel
                    {
                        CartId = cartUser.CartId,
                        ProductId = product.ProductId,
                        ItemSelected = true,
                        Quantity = quantityOrder,
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
                return Json(new { success = true, loginError = false, message = "Successfully Add Product." });
            }
            else
            {
                var myCart = Cart;
                var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
                var productView = _mapper.Map<ProductViewModel>(product);
                var productImage = _productImageRepo.GetItem().Where(p => p.ProductId == product.ProductId)
                    .Include(i => i.image).First();
                productView.ProductImageUrl = productImage.image.ImageUrl;
                if (product == null || product.Quantity == 0)
                {
                    return Json(new { success = false, loginError = false, message = "Out Of Stock" });
                }
                var checkItemExist = myCart.FirstOrDefault(i => i.ProductId == productId);
                if (checkItemExist == null)
                {
                    var item = new CartItemViewModel
                    {
                        ProductId = product.ProductId,
                        Quantity = quantityOrder,
                        Product = productView,
                        ItemSelected = true,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    myCart.Add(item);
                }
                else
                {
                    checkItemExist.Quantity++;
                }
                HttpContext.Session.Set(MyConst.CartKey, myCart);
                return Json(new { success = true, loginError = true, message = "Successfully Add Product." });
            }
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
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var quantityItemUpdate = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId && i.CartId == cartUser.CartId);
                quantityItemUpdate.Quantity++;
                if (quantityItemUpdate.Quantity > quantityAvailable)
                {
                    return Json(new { success = false, errorMessage = "The Quantity has reached the maximum." });
                }
                await _cartItemRepo.CommitAsync();
                return Json(new { success = true, errorMessage = "" });
            }
            else
            {
                var myCart = Cart;
                var quantityItemUpdate = myCart.FirstOrDefault(i => i.ProductId == product.ProductId);
                quantityItemUpdate.Quantity++;
                if (quantityItemUpdate.Quantity > quantityAvailable)
                {
                    return Json(new { success = false, errorMessage = "The Quantity has reached the maximum." });
                }
                HttpContext.Session.Set(MyConst.CartKey, myCart);
                return Json(new { success = true, errorMessage = "" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> MinusCartItemAsync(string productId)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == productId);
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.User.Claims.First().Value;
                var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                var quantityItemUpdate = _cartItemRepo.FirstOrDefault(i => i.ProductId == product.ProductId
                                                                                && i.CartId == cartUser.CartId);
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
            else
            {
                var myCart = Cart;
                var quantityItemUpdate = myCart.FirstOrDefault(i => i.ProductId == product.ProductId);
                if (quantityItemUpdate.Quantity == 1)
                {
                    quantityItemUpdate.Quantity = 1;
                }
                else
                {
                    quantityItemUpdate.Quantity--;
                }
                HttpContext.Session.Set(MyConst.CartKey, myCart);
                return Json(new { success = true, errorMessage = "" });
            }
        }
        public IActionResult SelectItem(string itemId)
        {
            if (User.Identity.IsAuthenticated)
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
            else
            {
                var myCart = Cart;
                var result = myCart.FirstOrDefault(i => i.CartItemId == itemId);
                if (result.ItemSelected == true)
                {
                    result.ItemSelected = false;
                }
                else
                {
                    result.ItemSelected = true;
                }
                HttpContext.Session.Set(MyConst.CartKey, myCart);
                return Ok();
            }
        }
        public IActionResult CheckOut()
        {
            if (TempData["RedirectedToSpecificAction"] != null && (bool)TempData["RedirectedToSpecificAction"])
            {
                // Clear the flag
                TempData["RedirectedToSpecificAction"] = false;
                // Redirect the user to SpecificAction again
                return RedirectToAction("HistoryOrder", "Order");
            }
            if (User.Identity.IsAuthenticated)
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
                ViewBag.ClientID = _paypalClient.ClientId;
                return View(userView);
            }
            else
            {
                var myCart = new CartViewModel();
                var myItems = Cart;
                myCart.CartItems = myItems;
                var userView = new UserViewModel();
                var ItemOrders = Cart.Where(i => i.ItemSelected == true).ToList();
                userView.Cart = myCart;
                ViewBag.ClientId = _paypalClient.ClientId;
                return View(userView);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(UserViewModel model)
        {
            try
            {
                decimal SubtotalOrder = 0;
                if (User.Identity.IsAuthenticated)
                {
                    var userId = HttpContext.User.Claims.First().Value;
                    var user = _userRepo.FirstOrDefault(u => u.UserId == userId);
                    var cartUser = _cartRepo.FirstOrDefault(c => c.CartId == user.CartId);
                    var ItemOrders = _cartItemRepo.GetItem().Where(i => i.ItemSelected == true && i.CartId == cartUser.CartId)
                                                .Include(p => p.product).ToList();
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
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Country = model.OrderViewModel.Country,
                        City = model.OrderViewModel.City,
                        State = model.OrderViewModel.State,
                        IsActive = true,
                        UserId = userId,
                        Address = model.OrderViewModel.Address,
                        TotalPrice = SubtotalOrder,
                        IsPaid = false,
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
                        //EmailSenderOrder(newOrder.OrderId);
                        //return RedirectToAction("Index", "Home");
                    }
                    TempData["RedirectedToSpecificAction"] = true;
                    return RedirectToAction("PaymentWithPaypal", new { orderId = newOrder.OrderId });
                }
                else
                {
                    var myCart = Cart;
                    foreach (var item in myCart)
                    {
                        var price = item.Quantity * item.Product.Price;
                        SubtotalOrder += (decimal)price;
                    }
                    var newOrder = new OrderCrudModel
                    {
                        OrderTitle = myCart.FirstOrDefault().Product.ProductName,
                        OrderCode = "ORD" + DateTime.Now.Ticks + "NLGY",
                        CreatedAt = DateTime.Now,
                        OrderStatus = 0,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Country = model.OrderViewModel.Country,
                        City = model.OrderViewModel.City,
                        State = model.OrderViewModel.State,
                        IsActive = true,
                        UserId = null,
                        Address = model.OrderViewModel.Address,
                        TotalPrice = SubtotalOrder,
                        IsPaid = false,
                    };
                    _orderRepo.Add(newOrder);
                    await _orderRepo.CommitAsync();
                    //Create OrderItem List
                    foreach (var item in myCart.ToList())
                    {
                        var orderItems = new OrderItemCrudModel
                        {
                            ProductId = item.ProductId,
                            Price = item.Quantity * item.Product.Price,
                            Quantity = item.Quantity,
                            OrderId = newOrder.OrderId,
                            CreatedAt = DateTime.Now,
                            IsActive = true,
                        };
                        _orderItemRepo.Add(orderItems);
                        await _orderItemRepo.CommitAsync();

                        //Remove CartItem && 
                        myCart.Remove(item);
                        HttpContext.Session.Set(MyConst.CartKey, myCart);

                        //Cập nhật số lượng hàng trong Kho.
                        var product = _productRepo.FirstOrDefault(p => p.ProductId == item.ProductId);
                        product.Quantity = product.Quantity - item.Quantity;
                    }
                    await _productRepo.CommitAsync();
                    //EmailSenderOrder(newOrder.OrderId);
                    //return RedirectToAction("CreatePaypalOrder", new { orderId = newOrder.OrderId });
                    TempData["RedirectedToSpecificAction"] = true;
                    return RedirectToAction("PaymentWithPaypal", new { orderId = newOrder.OrderId });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult PaymentWithPaypal(string Cancel = null, string blogId = "", string PayerID = "", string guid = "", string? orderId = "")
        {
           
            var checkId = orderId;
            if (string.IsNullOrEmpty(orderId))
            {
                checkId = (string)TempData["OrderId"].ToString();
            }
            //var order = new Models.Order();
            var order = _orderRepo.GetItem()
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.product)
                .FirstOrDefault(x => x.OrderId == checkId);
            var ClientID = _configuration.GetValue<string>("PaypalOptions:AppId");
            var ClientSecret = _configuration.GetValue<string>("PaypalOptions:AppSecret");
            var mode = _configuration.GetValue<string>("PaypalOptions:Mode");
            TempData["Id"] = orderId;
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
            try
            {
                string payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Cart/PaymentWithPayPal?";
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;
                    //
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId, order);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    _httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                    TempData["OrderId"] = order.OrderId.ToString();
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var paymentId = _httpContextAccessor.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("OrderHistory", "Order");
                    }
                    //Change Order
                    order.IsPaid = true;
                    _context.SaveChanges();
                    EmailSenderOrder(order.OrderId);
                    TempData.Clear();
                    var blogIds = executedPayment.transactions[0].item_list.items[0].sku;
                    return RedirectToAction("OrderHistory", "Order");
                }
            }
            catch (Exception ex)
            {
                return View("PaymentFailed");
            }
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {

            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId, Models.Order order)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            var totalPrice = string.Format(culture, "{0:0.00}", order.TotalPrice);
            var itemList = new ItemList()
            {
                items = new List<Item>(),
            };
            foreach (var i in order.OrderItems)
            {
                itemList.items.Add(new Item()
                {
                    name = i.product.ProductName,
                    currency = "USD",
                    price = string.Format(culture, "{0:0.00}", i.product.Price),
                    quantity = i.Quantity.ToString(),
                    sku = "asd"
                });
            }
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            //var details = new Details()
            //{
            //    tax = "1",
            //    shipping = "1",
            //    subtotal = "1"
            //};
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = totalPrice, // Total must be equal to sum of tax, shipping and subtotal.  
                //details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = order.OrderTitle.ToString(), //De
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
        private string GetIdCode(string userId)
        {
            string[] parts = userId.Split('-');
            string secondPartWithHyphen = parts[1].ToUpper();
            return secondPartWithHyphen;
        }
        //Send Email Order
        private void EmailSenderOrder(string orderId)
        {
            EmailSender emailSender = new EmailSender();
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            var products = new StringBuilder();
            var order = _orderRepo.GetItem().Include(x => x.OrderItems).Where(x => x.OrderId == orderId).FirstOrDefault();
            if (order != null)
            {
                foreach (var item in order.OrderItems)
                {
                    products.Append(item.product.ProductName);
                }
                products.Length -= 3;
                var status = Enum.GetName(typeof(EnumClass.OrderStatus), OrderStatus.Pending);
                var PriceDisplay = string.Format(culture, "{0:c}", order.TotalPrice);
                var pricePay = string.Format(culture, "{0:c}", order.TotalPrice + 6);
                var subject = "Order Confirmation";
                string body = $"Dear {order.FirstName},\n\n" +
                                   $"Order #{order.OrderCode}\n" +
                                   $" Your order is successfully created.\n" +
                                   $"Thank you for your order!\n\n" +
                                   $"Order Details:\n" +
                                   $"Product: {products}\n" +
                                   //$"Quantity: {quantity}\n" +
                                   $"Total Price of Product: {PriceDisplay}\n" +
                                   $"Shipping Fee $6 \n" +
                                   $"Total Payment: {pricePay}\n" +
                                   $"Order Status: {status}\n\n" +
                                   $"We appreciate your business.\n\n" +
                                   $"Sincerely,\n" +
                                   $"Your Company Name";
                emailSender.SendEmail(order.Email, subject, body);
            }
        }
    }
}
