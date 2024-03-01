using AutoMapper;
using Ecommerce.Const;
using Ecommerce.Extensions;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;
using static Ecommerce.Const.EnumClass;
using SendGrid.Helpers.Mail;
using System.Net.WebSockets;
using SendGrid.Helpers.Mail.Model;

namespace Ecommerce.Controllers
{
    public class UserController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly UserRepository _userRepo;
        private readonly CartRepository _cartRepo;
        private readonly IMapper _mapper;
        public UserController(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _userRepo = new UserRepository(_context, _mapper);
            _cartRepo = new CartRepository(_context, _mapper);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCrudModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userExisting = await _userRepo.FirstOrDefault(model);
                    if (userExisting != null)
                    {
                        return View(userExisting);
                    }
                    var cartUser = new CartCrudModel
                    {
                        TotalPrice = 0,
                    };
                    var newUser = new UserCrudModel
                    {
                        Email = model.Email,
                        Password = model.Password.Hash(),
                        Name = model.Name,
                        Gender = model.Gender,
                        Role = (int)EnumClass.Role.User,
                        CartId = cartUser.CartId,
                        Phone = model.Phone,
                        IsActive = true,
                        IsDeleted = false,
                        AccountActivated = false,
                    };
                    _cartRepo.Add(cartUser);
                    _userRepo.Add(newUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                else
                {
                    model.ErrorMessage = "Có lỗi khi tạo tài khoản";
                    return View(model);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ActivateAccount()
        {
            EmailSending emailSender = new EmailSending("josiahewalshu@hotmail.com", "jjosi123");
            emailSender.SendEmail("dominicculen@gmail.com", "Activate", "KKKKKKKKEEEEEEEE");
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("ReturnUrl", Request.Headers["Referer"].ToString());
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var result = await LoginValid(model);
                var returnUrl = HttpContext.Session.GetString("ReturnUrl");
                HttpContext.Session.Remove("ReturnUrl");
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect("/");
                }
                else
                {
                    var uriBuilder = new UriBuilder(returnUrl);
                    var queryParams = uriBuilder.Query.Contains("userId");
                    //var queryParams = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    if (queryParams)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(returnUrl);
                }
            }
            catch (Exception)
            {
                model.ErrorMessage = "Đăng nhập thất bại";
                return View(model);
            }
        }
        private async Task<UserViewModel> LoginValid(LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == model.Email.Trim().ToLower() 
                                                            && u.Password == model.Password.Hash());
            try
            {
                if(user != null)
                {
                    var login = CreateAuthenication(user);
                    var claims = new List<Claim>()
                    {
                        new Claim("UserId", login.UserId),
                        new Claim(ClaimTypes.Name, login.Name),
                        new Claim(ClaimTypes.Email, login.Email),
                        new Claim(ClaimTypes.Gender, ((EnumClass.Gender)login.Gender).ToString()),
                        new Claim(ClaimTypes.Role, ((EnumClass.Role)login.Role).ToString()),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                    {
                        IsPersistent = model.Remember
                    });
                    return login;
                }
                else
                {
                    throw new Exception("Đăng nhập thất bại");
                }
            }
            catch (Exception )
            {

                throw ;
            }
        }
        private UserViewModel CreateAuthenication(User user)
        {
            var userLogin = new UserViewModel
            {
                Email = user.Email,
                Gender = user.Gender,
                Name = user.Name,
                Role = user.Role,
            };
            return userLogin;
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetString("ReturnUrl", Request.Headers["Referer"].ToString());
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var returnUrl = HttpContext.Session.GetString("ReturnUrl");
            return RedirectToAction("Login");
        }

    }
}
