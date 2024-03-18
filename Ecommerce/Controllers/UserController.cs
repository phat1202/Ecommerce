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
using Ecommerce.Helpers;
using System.Linq;

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
        public IActionResult IndexAsync()
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
                    var userExisting = await _userRepo.CheckUserExist(model);
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
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        Role = (int)EnumClass.Role.User,
                        CartId = cartUser.CartId,
                        Phone = model.Phone,
                        IsActive = true,
                        IsDeleted = false,
                        AccountActivated = false,
                        ActivateToken = Guid.NewGuid(),
                    };
                    _cartRepo.Add(cartUser);
                    _userRepo.Add(newUser);
                    await _context.SaveChangesAsync();
                    EmailActivateAccount(newUser.Email, newUser.ActivateToken);
                    return RedirectToAction("Login");
                }
                else
                {
                    model.ErrorMessage = "Something goes wrong, please try again.";
                    return View(model);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Send Email Confirm
        private void EmailActivateAccount(string emailUser, Guid token)
        {
            EmailSender emailSender = new EmailSender();
            var user = _userRepo.FirstOrDefault(u => u.Email == emailUser);
            string activationUrl = $"https://localhost:7063/user/activate?token={token}";
            var subject = "Account Activation Request";
            var body = $"Dear {user.FirstName},\n\nPlease click the following link to activate your account:\n\n{activationUrl}\n\nThank you!";
            emailSender.SendEmail(emailUser, subject, body);
        }
        // Confirm Email
        public async Task<IActionResult> ActivateAsync(string token)
        {
            var user = _userRepo.FirstOrDefault(u => u.ActivateToken.ToString().Contains(token));
            if(user == null || user.AccountActivated == true)
            {
                return NotFound("InValid Url");
            }
            user.AccountActivated = true;
            await _userRepo.CommitAsync();
            return RedirectToAction("Login");
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
                if (result == null)
                {
                    model.ErrorMessage = "Login Failed";
                    return View(model);
                }
                if(result.AccountActivated == false)
                {
                    model.ErrorMessage = "Your Account have not been activated.";
                    return View(model);
                }
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
                model.ErrorMessage = "Login Failed";
                return View(model);
            }
        }
        private async Task<UserViewModel> LoginValid(LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == model.Email.Trim().ToLower()
                                                            && u.Password == model.Password.Hash());
            try
            {
                if (user != null)
                {
                    var login = CreateAuthenication(user);
                    var claims = new List<Claim>()
                    {
                        new Claim("UserId", login.UserId),
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
                    //throw new Exception("Đăng nhập thất bại");
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private UserViewModel CreateAuthenication(User user)
        {
            var userLogin = new UserViewModel
            {
                UserId = user.UserId,
                Email = user.Email,
                Gender = user.Gender,
                Role = user.Role,
                AccountActivated = user.AccountActivated,
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
